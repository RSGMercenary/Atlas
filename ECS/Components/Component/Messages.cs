using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Components.Component
{
	#region Interfaces

	public interface IManagerAddMessage<out T> : IKeyValueMessage<T, int, IEntity> where T : IComponent { }

	public interface IManagerRemoveMessage<out T> : IKeyValueMessage<T, int, IEntity> where T : IComponent { }

	public interface IManagerMessage<out T> : IMessage<T> where T : IComponent { }

	#endregion

	#region Classes

	class ManagerAddMessage<T> : KeyValueMessage<T, int, IEntity>, IManagerAddMessage<T> where T : IComponent
	{
		public ManagerAddMessage(int key, IEntity value) : base(key, value) { }
	}

	class ManagerRemoveMessage<T> : KeyValueMessage<T, int, IEntity>, IManagerRemoveMessage<T> where T : IComponent
	{
		public ManagerRemoveMessage(int key, IEntity value) : base(key, value) { }
	}

	class ManagerMessage<T> : Message<T>, IManagerMessage<T> where T : IComponent
	{
		public ManagerMessage() { }
	}

	#endregion
}