using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Messages
{
	interface IComponentRemoveMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
