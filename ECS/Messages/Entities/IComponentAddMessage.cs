using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using System;

namespace Atlas.Framework.Messages
{
	public interface IComponentAddMessage : IKeyValueMessage<IEntity, Type, IComponent>
	{

	}
}
