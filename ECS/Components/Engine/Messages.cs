using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine
{
	#region Interfaces

	public interface IEntityAddMessage : IValueMessage<IEngine, IEntity> { }

	public interface IEntityRemoveMessage : IValueMessage<IEngine, IEntity> { }

	public interface IFamilyAddMessage : IKeyValueMessage<IEngine, Type, IFamily> { }

	public interface IFamilyRemoveMessage : IKeyValueMessage<IEngine, Type, IFamily> { }

	public interface ISystemAddMessage : IKeyValueMessage<IEngine, Type, ISystem> { }

	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, ISystem> { }

	#endregion

	#region Classes

	class EntityAddMessage : ValueMessage<IEngine, IEntity>, IEntityAddMessage
	{
		public EntityAddMessage(IEntity value) : base(value) { }
	}

	class EntityRemoveMessage : ValueMessage<IEngine, IEntity>, IEntityRemoveMessage
	{
		public EntityRemoveMessage(IEntity value) : base(value) { }
	}

	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(Type key, IFamily value) : base(key, value) { }
	}

	class FamilyRemoveMessage : KeyValueMessage<IEngine, Type, IFamily>, IFamilyRemoveMessage
	{
		public FamilyRemoveMessage(Type key, IFamily value) : base(key, value) { }
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