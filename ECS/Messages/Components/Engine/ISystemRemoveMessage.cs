using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Messages
{
	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, IReadOnlySystem>
	{

	}
}
