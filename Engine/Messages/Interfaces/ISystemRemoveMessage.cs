using Atlas.Engine.Components;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Messages
{
	interface ISystemRemoveMessage : IKeyValueMessage<IEngine, Type, ISystem>
	{

	}
}
