using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine
{
	#region Interfaces
	public interface IEngineMessage<out T> : IPropertyMessage<T, IEngine> where T : IEngineObject, IMessenger { }

	public interface IEntityAddMessage : IValueMessage<IEngine, IEntity> { }

	public interface IEntityRemoveMessage : IValueMessage<IEngine, IEntity> { }

	public interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily> { }

	public interface IFamilyRemoveMessage : IKeyValueMessage<IEngine, Type, IReadOnlyFamily> { }

	public interface ISystemAddMessage : IKeyValueMessage<IEngine, Type, ISystem> { }

	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, ISystem> { }
	#endregion

	#region Classes
	class EngineMessage<T> : PropertyMessage<T, IEngine>, IEngineMessage<T> where T : IEngineObject, IMessenger
	{
		public EngineMessage(IEngine current, IEngine previous) : base(current, previous) { }
	}

	class EntityAddMessage : ValueMessage<IEngine, IEntity>, IEntityAddMessage
	{
		public EntityAddMessage(IEntity value) : base(value) { }
	}

	class EntityRemoveMessage : ValueMessage<IEngine, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEntity value) : base(value) { }
	}

	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(Type key, IReadOnlyFamily value) : base(key, value) { }
	}

	class FamilyRemoveMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyRemoveMessage
	{
		public FamilyRemoveMessage(Type key, IReadOnlyFamily value) : base(key, value) { }
	}

	class SystemAddMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemAddMessage
	{
		public SystemAddMessage(Type key, ISystem value) : base(key, value) { }
	}

	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(Type key, ISystem value) : base(key, value) { }
	}
	#endregion
}