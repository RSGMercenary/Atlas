using Atlas.Core.Messages;
using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities
{
	#region Interfaces

	public interface IComponentAddMessage : IKeyValueMessage<IEntity, Type, IComponent> { }

	public interface IComponentRemoveMessage : IKeyValueMessage<IEntity, Type, IComponent> { }

	public interface IGlobalNameMessage : IPropertyMessage<IEntity, string> { }

	public interface ILocalNameMessage : IPropertyMessage<IEntity, string> { }

	public interface IFreeSleepMessage : IPropertyMessage<IEntity, int> { }

	#endregion

	#region Classes

	class ComponentAddMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentAddMessage
	{
		public ComponentAddMessage(Type key, IComponent value) : base(key, value) { }
	}

	class ComponentRemoveMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentRemoveMessage
	{
		public ComponentRemoveMessage(Type key, IComponent value) : base(key, value) { }
	}

	class GlobalNameMessage : PropertyMessage<IEntity, string>, IGlobalNameMessage
	{
		public GlobalNameMessage(string current, string previous) : base(current, previous) { }
	}

	class LocalNameMessage : PropertyMessage<IEntity, string>, ILocalNameMessage
	{
		public LocalNameMessage(string current, string previous) : base(current, previous) { }
	}

	class FreeSleepMessage : PropertyMessage<IEntity, int>, IFreeSleepMessage
	{
		public FreeSleepMessage(int current, int previous) : base(current, previous) { }
	}

	#endregion
}