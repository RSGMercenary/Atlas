using Atlas.Core.Collections.Group;
using Atlas.ECS.Messages;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components
{
	public class SystemManager : AtlasComponent<ISystemManager>, ISystemManager
	{
		private readonly Group<Type> systems = new Group<Type>();

		public SystemManager()
		{

		}

		protected override void Disposing(bool finalizer)
		{
			if(!finalizer)
			{
				RemoveSystems();
			}
			base.Disposing(finalizer);
		}

		public SystemManager(IEnumerable<Type> types)
		{
			foreach(var type in types)
				AddSystem(type);
		}

		protected override void ChangingEngine(IEngine current, IEngine previous)
		{
			if(current != null)
			{
				foreach(var type in systems)
					current.AddSystem(type);
			}
			else
			{
				foreach(var type in systems)
					previous.RemoveSystem(type);
			}
			base.ChangingEngine(current, previous);
		}

		public IReadOnlyGroup<Type> Systems
		{
			get { return systems; }
		}

		public bool HasSystem<TISystem>() where TISystem : ISystem
		{
			return HasSystem(typeof(TISystem));
		}

		public bool HasSystem(Type type)
		{
			return systems.Contains(type);
		}

		public bool AddSystem<TISystem>() where TISystem : ISystem
		{
			return AddSystem(typeof(TISystem));
		}

		public bool AddSystem(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface) //Type must be an interface.
				return false;
			if(type == typeof(ISystem)) //Type can't directly be ISystem.
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type)) //Type must be a subclass of ISystem.
				return false;
			if(systems.Contains(type))
				return false;
			systems.Add(type);
			Engine?.AddSystem(type);
			Message<ISystemTypeAddMessage>(new SystemTypeAddMessage(this, type));
			return true;
		}

		public bool RemoveSystem<TISystem>() where TISystem : ISystem
		{
			return RemoveSystem(typeof(TISystem));
		}

		public bool RemoveSystem(Type type)
		{
			if(type == null)
				return false;
			if(!systems.Contains(type))
				return false;
			systems.Remove(type);
			Engine?.RemoveSystem(type);
			Message<ISystemTypeRemoveMessage>(new SystemTypeRemoveMessage(this, type));
			return true;
		}

		public bool RemoveSystems()
		{
			if(systems.Count <= 0)
				return false;
			foreach(var system in systems)
				RemoveSystem(system);
			return true;
		}
	}
}