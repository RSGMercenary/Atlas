using Atlas.Core.Messages;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Messages
{
	public interface ISystemAddMessage : IKeyValueMessage<IEngine, Type, ISystem>
	{

	}
}