using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.Core.Messages
{
	public interface IComponentAddMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
