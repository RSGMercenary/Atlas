using Atlas.Engine.Components;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Messages
{
	public interface ISystemAddMessage : IKeyValueMessage<IEngine, Type, ISystem>
	{

	}
}
