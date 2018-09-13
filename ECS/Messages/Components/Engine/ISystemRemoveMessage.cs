using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.Core.Messages
{
	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, IReadOnlySystem>
	{

	}
}
