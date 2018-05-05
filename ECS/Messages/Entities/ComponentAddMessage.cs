using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.Framework.Messages
{
	class ComponentAddMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentAddMessage
	{
		public ComponentAddMessage(Type key, IComponent value) : base(key, value)
		{
		}
	}
}
