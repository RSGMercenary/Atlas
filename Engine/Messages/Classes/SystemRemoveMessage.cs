using Atlas.Engine.Components;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Messages
{
	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(Type key, ISystem value) : base(key, value)
		{
		}
	}
}
