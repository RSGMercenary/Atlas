using Atlas.Core.Messages;
using Atlas.ECS.Components;
using System;

namespace Atlas.ECS.Entities.Messages
{
	public interface IComponentAddMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}