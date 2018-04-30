using Atlas.Engine.Components;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Messages
{
	public interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, ISystem>
	{

	}
}
