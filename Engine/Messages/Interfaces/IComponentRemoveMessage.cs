using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Messages
{
	public interface IComponentRemoveMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
