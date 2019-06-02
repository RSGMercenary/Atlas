using Atlas.Core.Messages;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Messages
{
	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, ISystem>
	{

	}
}