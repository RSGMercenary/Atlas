using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.Framework.Messages
{
	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, IReadOnlySystem>
	{

	}
}
