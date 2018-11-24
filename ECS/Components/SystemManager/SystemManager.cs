using Atlas.Core.Collections.Group;
using Atlas.ECS.Components.Messages;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components
{
	public class SystemManager : AtlasComponent, ISystemManager
	{
		private readonly Group<Type> types = new Group<Type>();

		public SystemManager()
		{

		}

		public SystemManager(params Type[] types) : this(types as IEnumerable<Type>)
		{
			foreach(var type in types)
				AddSystem(type);
		}

		public SystemManager(IEnumerable<Type> types)
		{
			foreach(var type in types)
				AddSystem(type);
		}

		protected override void Disposing(bool finalizer)
		{
			if(!finalizer)
			{
				RemoveSystems();
			}
			base.Disposing(finalizer);
		}

		protected override void ChangingEngine(IEngine current, IEngine previous)
		{
			if(current != null)
			{
				foreach(var type in types)
					current.AddSystem(type);
			}
			else
			{
				foreach(var type in types)
					previous.RemoveSystem(type);
			}
			base.ChangingEngine(current, previous);
		}

		public IReadOnlyGroup<Type> Systems
		{
			get { return types; }
		}

		public bool HasSystem<TISystem>() where TISystem : ISystem
		{
			return HasSystem(typeof(TISystem));
		}

		public bool HasSystem(Type type)
		{
			return types.Contains(type);
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
			if(types.Contains(type))
				return false;
			types.Add(type);
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
			if(!types.Contains(type))
				return false;
			types.Remove(type);
			Engine?.RemoveSystem(type);
			Message<ISystemTypeRemoveMessage>(new SystemTypeRemoveMessage(this, type));
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