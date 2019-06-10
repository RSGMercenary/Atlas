using Atlas.Core.Collections.Group;
using Atlas.ECS.Components.Messages;
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

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			foreach(var type in types)
				engine.AddSystem(type);
		}

		protected override void RemovingEngine(IEngine engine)
		{
			foreach(var type in types)
				engine.RemoveSystem(type);
			base.RemovingEngine(engine);
		}

		public IReadOnlyGroup<Type> Systems
		{
			get { return types; }
		}

		public bool HasSystem<TKey>()
			where TKey : class, ISystem, new()
		{
			return HasSystem(typeof(TKey));
		}

		public bool HasSystem(Type type)
		{
			return types.Contains(type);
		}

		public bool AddSystem<TKey>()
			where TKey : class, ISystem, new()
		{
			return AddSystem(typeof(TKey));
		}

		public bool AddSystem(Type type)
		{
			if(type == null)
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type)) //Type must be a subclass of ISystem.
				return false;
			if(types.Contains(type))
				return false;
			types.Add(type);
			Engine?.AddSystem(type);
			Message<ISystemTypeAddMessage>(new SystemTypeAddMessage(type));
			return true;
		}

		public bool RemoveSystem<TKey>()
			where TKey : class, ISystem, new()
		{
			return RemoveSystem(typeof(TKey));
		}

		public bool RemoveSystem(Type type)
		{
			if(type == null)
				return false;
			if(!types.Contains(type))
				return false;
			types.Remove(type);
			Engine?.RemoveSystem(type);
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
	}
}