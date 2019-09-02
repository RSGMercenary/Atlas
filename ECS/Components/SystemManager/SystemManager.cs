using Atlas.Core.Collections.Group;
using Atlas.ECS.Components.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects.Messages;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components
{
	public class SystemManager : AtlasComponent<ISystemManager>, ISystemManager
	{
		private readonly Group<Type> types = new Group<Type>();

		public SystemManager() { }

		public SystemManager(params Type[] types) : this(types as IEnumerable<Type>) { }

		public SystemManager(IEnumerable<Type> types)
		{
			foreach(var type in types)
				AddSystem(type);
		}

		protected override void Disposing()
		{
			RemoveSystems();
			base.Disposing();
		}

		#region Engine/Systems

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			entity.AddListener<IEngineMessage<IEntity>>(UpdateSystems);
			UpdateSystems(entity.Engine, true);
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			entity.RemoveListener<IEngineMessage<IEntity>>(UpdateSystems);
			UpdateSystems(entity.Engine, false);
			base.RemovingManager(entity, index);
		}

		private void UpdateSystems(IEngineMessage<IEntity> message)
		{
			UpdateSystems(message.PreviousValue, false);
			UpdateSystems(message.CurrentValue, true);
		}

		private void UpdateSystems(IEngine engine, bool add)
		{
			if(engine == null)
				return;
			foreach(var type in types)
			{
				if(add)
					engine.AddSystem(type);
				else
					engine.RemoveSystem(type);
			}
		}

		#endregion

		#region Get

		public IReadOnlyGroup<Type> Systems => types;

		#endregion

		#region Has

		public bool HasSystem<TKey>() where TKey : class, ISystem, new() => HasSystem(typeof(TKey));

		public bool HasSystem(Type type) => types.Contains(type);

		#endregion

		#region Add

		public bool AddSystem<TKey>() where TKey : class, ISystem, new() => AddSystem(typeof(TKey));

		public bool AddSystem(Type type)
		{
			if(type == null)
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type)) //Type must be a subclass of ISystem.
				return false;
			if(types.Contains(type))
				return false;
			types.Add(type);
			Manager?.Engine?.AddSystem(type);
			Message<ISystemTypeAddMessage>(new SystemTypeAddMessage(type));
			return true;
		}

		#endregion

		#region Remove

		public bool RemoveSystem<TKey>() where TKey : class, ISystem, new() => RemoveSystem(typeof(TKey));

		public bool RemoveSystem(Type type)
		{
			if(type == null)
				return false;
			if(!types.Contains(type))
				return false;
			types.Remove(type);
			Manager?.Engine?.RemoveSystem(type);
			Message<ISystemTypeRemoveMessage>(new SystemTypeRemoveMessage(type));
			return true;
		}

		public bool RemoveSystems()
		{
			if(types.Count <= 0)
				return false;
			foreach(var type in types)
				RemoveSystem(type);
			return true;
		}

		#endregion
	}
}