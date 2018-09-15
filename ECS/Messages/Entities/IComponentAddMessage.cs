using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Messages
{
	public interface IComponentAddMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
