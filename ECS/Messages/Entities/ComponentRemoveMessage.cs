using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.Framework.Messages
{
	class ComponentRemoveMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentRemoveMessage
	{
		public ComponentRemoveMessage(Type key, IComponent value) : base(key, value)
		{
		}
	}
}
