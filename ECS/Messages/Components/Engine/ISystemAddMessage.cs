using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.Core.Messages
{
	public interface ISystemAddMessage : IKeyValueMessage<IEngine, Type, IReadOnlySystem>
	{

	}
}
