using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Messages
{
	class ComponentAddMessage : KeyValueMessage<IEntity, Type, IComponent>, IComponentAddMessage
	{
		public ComponentAddMessage(Type key, IComponent value) : base(key, value)
		{
		}
	}
}
