using Atlas.Core.Messages;
using Atlas.ECS.Components;
using System;

namespace Atlas.ECS.Entities.Messages
{
	class ComponentRemoveMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentRemoveMessage
	{
		public ComponentRemoveMessage(IEntity messenger, Type key, IComponent value) : base(messenger, key, value)
		{
		}
	}
}
