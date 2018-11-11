using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Messages
{
	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(IEngine messenger, Type key, ISystem value) : base(messenger, key, value)
		{
		}
	}
}
