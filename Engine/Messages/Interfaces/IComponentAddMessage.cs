using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Messages
{
	interface IComponentAddMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
