using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Messages;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.ECS.Components
{
	public sealed class AtlasEngine : AtlasComponent<IEngine>, IEngine
	{
		#region Static Singleton

		private static AtlasEngine instance;

		/// <summary>
		/// Creates a singleton instance of the Engine.
		/// Only one Engine should exist at a time.
		/// </summary>
		public static AtlasEngine Instance
		{
			get { return instance = instance ?? new AtlasEngine(); }
		}

		#endregion

		private Group<IEntity> entities = new Group<IEntity>();
		private Group<IFamily> families = new Group<IFamily>();
		private Group<ISystem> systems = new Group<ISystem>();

		private Dictionary<string, IEntity> entitiesGlobalName = new Dictionary<string, IEntity>();
		private Dictionary<Type, IFamily> familiesType = new Dictionary<Type, IFamily>();
		private Dictionary<Type, ISystem> systemsType = new Dictionary<Type, ISystem>();

		private Dictionary<Type, Type> systemsInstance = new Dictionary<Type, Type>();

		private Dictionary<Type, int> familiesReference = new Dictionary<Type, int>();
		private Dictionary<Type, int> systemsReference = new Dictionary<Type, int>();

		private Stack<IReadOnlyFamily> familiesRemoved = new Stack<IReadOnlyFamily>();
		private Stack<IReadOnlySystem> systemsRemoved = new Stack<IReadOnlySystem>();

		private Dictionary<double, int> fixedTimesReference = new Dictionary<double, int>();
		private Dictionary<double, double> fixedTimesTotal = new Dictionary<double, double>();
		private Dictionary<double, int> fixedTimesUpdate = new Dictionary<double, int>();

		private Stopwatch timer = new Stopwatch();
		private IReadOnlySystem currentSystem;

		private bool isRunning = false;
		private bool isUpdating = false;

		private double deltaTime = 0;
		private double totalTime = 0;

		private AtlasEngine()
		{

		}

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			entity.AddListener<IChildAddMessage>(EntityChildAdded, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IChildRemoveMessage>(EntityChildRemoved, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IGlobalNameMessage>(EntityGlobalNameChanged, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IComponentAddMessage>(EntityComponentAdded, int.MinValue, MessageHierarchy.All);
			entity.AddListener<IComponentRemoveMessage>(EntityComponentRemoved, int.MinValue, MessageHierarchy.All);
			entity.AddListener<ISystemTypeAddMessage>(EntitySystemAdded, int.MinValue, MessageHierarchy.All);
			entity.AddListener<ISystemTypeRemoveMessage>(EntitySystemRemoved, int.MinValue, MessageHierarchy.All);
			AddEntity(entity);
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			RemoveEntity(entity);
			entity.RemoveListener<IChildAddMessage>(EntityChildAdded);
			entity.RemoveListener<IChildRemoveMessage>(EntityChildRemoved);
			entity.RemoveListener<IGlobalNameMessage>(EntityGlobalNameChanged);
			entity.RemoveListener<IComponentAddMessage>(EntityComponentAdded);
			entity.RemoveListener<IComponentRemoveMessage>(EntityComponentRemoved);
			entity.RemoveListener<ISystemTypeAddMessage>(EntitySystemAdded);
			entity.RemoveListener<ISystemTypeRemoveMessage>(EntitySystemRemoved);
			base.RemovingManager(entity, index);
		}

		public IReadOnlyGroup<IEntity> Entities { get { return entities; } }
		public IReadOnlyGroup<IReadOnlyFamily> Families { get { return families; } }
		public IReadOnlyGroup<IReadOnlySystem> Systems { get { return systems; } }

		#region Entities

		public bool HasEntity(string globalName)
		{
			return !string.IsNullOrWhiteSpace(globalName) && entitiesGlobalName.ContainsKey(globalName);
		}

		public bool HasEntity(IEntity entity)
		{
			return entity != null && entitiesGlobalName.ContainsKey(entity.GlobalName) && entitiesGlobalName[entity.GlobalName] == entity;
		}

		public IEntity GetEntity(string globalName)
		{
			return entitiesGlobalName.ContainsKey(globalName) ? entitiesGlobalName[globalName] : null;
		}

		private void AddEntity(IEntity entity)
		{
			//Change the global name of an incoming Entity if it already exists.
			if(entitiesGlobalName.ContainsKey(entity.GlobalName))
				entity.GlobalName = AtlasEntity.UniqueName;

			entitiesGlobalName.Add(entity.GlobalName, entity);
			entities.Add(entity);
			entity.Engine = this;

			//TO-DO
			//entity.Systems isn't an EngineList, so it might screw up.
			foreach(var type in entity.Systems)
				AddSystem(type);

			foreach(var family in families)
				family.AddEntity(entity);

			Dispatch<IEntityAddMessage>(new EntityAddMessage(this, entity));

			foreach(var child in entity.Children.Forward())
				AddEntity(child);
		}

		private void RemoveEntity(IEntity entity)
		{
			//Protect against parents signaling a child being removed which never got to be added.
			if(!entitiesGlobalName.ContainsKey(entity.GlobalName) ||
				entitiesGlobalName[entity.GlobalName] != entity)
				return;

			foreach(var child in entity.Children.Backward())
				RemoveEntity(child);

			Dispatch<IEntityRemoveMessage>(new EntityRemoveMessage(this, entity));

			//TO-DO
			//entity.Systems isn't an EngineList, so it might screw up.
			foreach(var type in entity.Systems)
				RemoveSystem(type);

			foreach(var family in families)
				family.RemoveEntity(entity);

			entitiesGlobalName.Remove(entity.GlobalName);
			entities.Remove(entity);
			entity.Engine = null;
		}

		private void EntityChildAdded(IChildAddMessage message)
		{
			AddEntity(message.Value);
		}

		private void EntityChildRemoved(IChildRemoveMessage message)
		{
			if(message.Value.Parent == null)
				RemoveEntity(message.Value);
		}

		private void EntityGlobalNameChanged(IGlobalNameMessage message)
		{
			entitiesGlobalName.Remove(message.PreviousValue);
			entitiesGlobalName.Add(message.CurrentValue, message.Messenger);
		}

		#endregion

		#region Systems

		public bool AddSystemType<TISystem, TSystem>()
			where TISystem : ISystem
			where TSystem : TISystem, new()
		{
			return AddSystemType(typeof(TISystem), typeof(TSystem));
		}

		public bool AddSystemType(Type type, Type instance)
		{
			if(type == null)
				return false;
			if(instance == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(!instance.IsClass)
				return false;
			if(type == typeof(ISystem))
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!type.IsAssignableFrom(instance))
				return false;
			if(instance.GetConstructor(Type.EmptyTypes) == null)
				return false;
			if(systemsInstance.ContainsKey(type) && systemsInstance[type] == instance)
				return false;
			RemoveSystemType(type);
			systemsInstance.Add(type, instance);
			AddSystem(type);
			return true;
		}

		public bool RemoveSystemType<TISystem>()
			where TISystem : IReadOnlySystem
		{
			return RemoveSystemType(typeof(TISystem));
		}

		public bool RemoveSystemType(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(type == typeof(IReadOnlySystem))
				return false;
			if(!typeof(IReadOnlySystem).IsAssignableFrom(type))
				return false;
			if(!systemsInstance.ContainsKey(type))
				return false;
			systemsInstance.Remove(type);
			RemoveSystem(type);
			return true;
		}

		private void AddSystem(Type type)
		{
			if(!systemsReference.ContainsKey(type))
				return;
			//There's no system instance class assigned.
			if(!systemsInstance.ContainsKey(type))
				return;
			ISystem system;
			try
			{
				system = Activator.CreateInstance(systemsInstance[type]) as ISystem;
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
				return;
			}

			systemsType.Add(type, system);

			system.AddListener<IPriorityMessage>(SystemPriorityChanged);
			SystemPriorityChanged(system);

			system.AddListener<IFixedTimeMessage>(SystemFixedTimeChanged);
			AddFixedTime(system.FixedTime);

			system.Engine = this;
			Dispatch<ISystemAddMessage>(new SystemAddMessage(this, type, system));
		}

		private void RemoveSystem(Type type)
		{
			if(!systemsType.ContainsKey(type))
				return;
			var system = systemsType[type];

			system.RemoveListener<IFixedTimeMessage>(SystemFixedTimeChanged);
			RemoveFixedTime(system.FixedTime);

			system.RemoveListener<IPriorityMessage>(SystemPriorityChanged);
			systems.Remove(system);

			systemsType.Remove(type);
			Dispatch<ISystemRemoveMessage>(new SystemRemoveMessage(this, type, system));
			if(isUpdating)
			{
				systemsRemoved.Push(system);
			}
			else
			{
				system.Dispose();
			}
		}

		private void EntitySystemAdded(ISystemTypeAddMessage message)
		{
			if(!systemsReference.ContainsKey(message.Value))
			{
				systemsReference.Add(message.Value, 1);
				AddSystem(message.Value);
			}
			else
				++systemsReference[message.Value];
		}

		private void EntitySystemRemoved(ISystemTypeRemoveMessage message)
		{
			if(--systemsReference[message.Value] > 0)
				return;
			systemsReference.Remove(message.Value);
			RemoveSystem(message.Value);
		}

		private void SystemPriorityChanged(IPriorityMessage message)
		{
			SystemPriorityChanged(message.Messenger as ISystem);
		}

		private void SystemPriorityChanged(ISystem system)
		{
			systems.Remove(system);

			for(var index = systems.Count; index > 0; --index)
			{
				if(systems[index - 1].Priority <= system.Priority)
				{
					systems.Insert(index, system);
					return;
				}
			}

			systems.Insert(0, system);
		}

		private void SystemFixedTimeChanged(IFixedTimeMessage message)
		{
			RemoveFixedTime(message.PreviousValue);
			AddFixedTime(message.CurrentValue);
		}

		private void AddFixedTime(double fixedTime)
		{
			if(fixedTime <= 0)
				return;
			if(!fixedTimesReference.ContainsKey(fixedTime))
			{
				fixedTimesReference.Add(fixedTime, 1);
				fixedTimesTotal.Add(fixedTime, 0);
				//Get the total time in sync with the rest of the engine.
				while(fixedTimesTotal[fixedTime] + fixedTime < totalTime)
					fixedTimesTotal[fixedTime] += fixedTime;
				fixedTimesUpdate.Add(fixedTime, 0);
			}
			else
			{
				++fixedTimesReference[fixedTime];
			}
		}

		private void RemoveFixedTime(double fixedTime)
		{
			if(--fixedTimesReference[fixedTime] > 0)
				return;
			fixedTimesReference.Remove(fixedTime);
			fixedTimesTotal.Remove(fixedTime);
			fixedTimesUpdate.Remove(fixedTime);
		}

		public bool HasSystem(IReadOnlySystem system)
		{
			return systems.Contains(system as ISystem);
		}

		public bool HasSystem<TISystem>() where TISystem : IReadOnlySystem
		{
			return HasSystem(typeof(TISystem));
		}

		public bool HasSystem(Type type)
		{
			return systemsType.ContainsKey(type);
		}

		public TISystem GetSystem<TISystem>() where TISystem : IReadOnlySystem
		{
			return (TISystem)GetSystem(typeof(TISystem));
		}

		public IReadOnlySystem GetSystem(Type type)
		{
			return systemsType.ContainsKey(type) ? systemsType[type] : null;
		}

		public IReadOnlySystem GetSystem(int index)
		{
			return systems[index];
		}

		#endregion

		#region Updates

		public double DeltaTime
		{
			get { return deltaTime; }
			private set
			{
				if(deltaTime == value)
					return;
				deltaTime = value;
			}
		}

		public double TotalTime
		{
			get { return totalTime; }
			private set
			{
				if(totalTime == value)
					return;
				totalTime = value;
			}
		}

		public IReadOnlyDictionary<double, double> FixedTimes
		{
			get { return fixedTimesTotal; }
		}

		public bool IsUpdating
		{
			get { return isUpdating; }
			private set
			{
				if(isUpdating == value)
					return;
				var previous = isUpdating;
				isUpdating = value;
				Dispatch<IUpdateMessage<IEngine>>(new UpdateMessage<IEngine>(this, value, previous));
			}
		}

		public IReadOnlySystem CurrentSystem
		{
			get { return currentSystem; }
			private set
			{
				if(currentSystem == value)
					return;
				//If a Signal/Message were to ever be put here, do it before the set.
				//Prevents System.Update() from being mis-called.
				currentSystem = value;
			}
		}

		public bool IsRunning
		{
			get { return isRunning; }
			set
			{
				if(isRunning == value)
					return;
				isRunning = value;
				//Only run again when the last Update()/timer is done.
				//If the Engine is turned off and on during an Update()
				//loop, while(isRunning) will catch it.
				if(value && !timer.IsRunning)
				{
					timer.Restart();
					var previousTime = 0d;
					while(isRunning)
					{
						var currentTime = timer.Elapsed.TotalSeconds;
#if DEBUG
						//This is mainly for debugging.
						//Stopwatch is constant and doesn't stop for breakpoints.
						if(currentTime - previousTime > 1)
						{
							//Ignore the elapsed time while breakpoints were active.
							previousTime = currentTime;
							continue;
						}
#endif
						//Update delta time and total time.
						DeltaTime = currentTime - previousTime;
						TotalTime += deltaTime;

						//Update fixed total times and calculate number of fixed updates for this loop.
						//Have to copy the keys, or you get modification exceptions.
						foreach(var fixedTime in new List<double>(fixedTimesUpdate.Keys))
						{
							fixedTimesUpdate[fixedTime] = 0;
							while(fixedTimesTotal[fixedTime] + fixedTime < totalTime)
							{
								fixedTimesTotal[fixedTime] += fixedTime;
								++fixedTimesUpdate[fixedTime];
							}
						}

						IsUpdating = true;

						foreach(var system in systems)
						{
							//System is reactive/listens for messages and does not receive time-related updates.
							if(system.FixedTime == 0)
								continue;
							//System receives updates.
							if(system.FixedTime > 0)
							{
								//Store system.FixedTime locally in case updates alter the fixed time.
								//Probably an extremely rare exception, but still.
								var fixedTime = system.FixedTime;

								//Update System based on fixed time.
								CurrentSystem = system;
								for(var i = fixedTimesUpdate[fixedTime]; i > 0; --i)
									system.Update(fixedTime);
								CurrentSystem = null;
							}
							else
							{
								//Update System based on delta time.
								CurrentSystem = system;
								system.Update(deltaTime);
								CurrentSystem = null;
							}
						}

						IsUpdating = false;

						DestroySystems();
						DestroyFamilies();

						previousTime = currentTime;
					}
					DeltaTime = 0;
					timer.Stop();
				}
			}
		}

		private void DestroySystems()
		{
			while(systemsRemoved.Count > 0)
				systemsRemoved.Pop().Dispose();
		}

		#endregion

		#region Families

		public bool HasFamily(IReadOnlyFamily family)
		{
			return familiesType.ContainsValue(family as IFamily);
		}

		public bool HasFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			return HasFamily(typeof(TFamilyMember));
		}

		public bool HasFamily(Type type)
		{
			return familiesType.ContainsKey(type);
		}

		public IReadOnlyFamily<TFamilyMember> AddFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
			{
				var family = new AtlasFamily<TFamilyMember>();

				families.Add(family);
				familiesType.Add(type, family);
				familiesReference.Add(type, 1);
				family.Engine = this;

				foreach(var entity in entities)
					family.AddEntity(entity);
				Dispatch<IFamilyAddMessage>(new FamilyAddMessage(this, type, family));
				return family;
			}
			else
			{
				++familiesReference[type];
				return familiesType[type] as IFamily<TFamilyMember>;
			}
		}

		public IReadOnlyFamily<TFamilyMember> RemoveFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			var type = typeof(TFamilyMember);
			if(!familiesType.ContainsKey(type))
				return null;
			var family = familiesType[type];
			if(--familiesReference[type] > 0)
				return family as IReadOnlyFamily<TFamilyMember>;
			families.Remove(family);
			familiesType.Remove(type);
			familiesReference.Remove(type);
			Dispatch<IFamilyRemoveMessage>(new FamilyRemoveMessage(this, type, family));
			if(isUpdating)
			{
				familiesRemoved.Push(family);
			}
			else
			{
				family.Dispose();
			}
			return family as IReadOnlyFamily<TFamilyMember>;
		}

		private void DestroyFamilies()
		{
			while(familiesRemoved.Count > 0)
				familiesRemoved.Pop().Dispose();
		}

		public IReadOnlyFamily<TFamilyMember> GetFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new()
		{
			return GetFamily(typeof(TFamilyMember)) as IFamily<TFamilyMember>;
		}

		public IReadOnlyFamily GetFamily(Type type)
		{
			return familiesType.ContainsKey(type) ? familiesType[type] : null;
		}

		private void EntityComponentAdded(IComponentAddMessage message)
		{
			foreach(var family in families)
				family.AddEntity(message.Messenger, message.Key);
		}

		private void EntityComponentRemoved(IComponentRemoveMessage message)
		{
			foreach(var family in families)
				family.RemoveEntity(message.Messenger, message.Key);
		}

		#endregion
	}
}