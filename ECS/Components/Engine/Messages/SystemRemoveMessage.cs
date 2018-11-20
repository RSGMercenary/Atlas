using Atlas.Core.Messages;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(IEngine messenger, Type key, ISystem value) : base(messenger, key, value)
		{
		}
	}
}
