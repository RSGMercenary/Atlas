using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Messages
{
	class ComponentRemoveMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentRemoveMessage
	{
		public ComponentRemoveMessage(IEntity messenger, Type key, IComponent value) : base(messenger, key, value)
		{
		}
	}
}
