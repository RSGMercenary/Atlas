using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.Framework.Messages
{
	public interface IComponentRemoveMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
