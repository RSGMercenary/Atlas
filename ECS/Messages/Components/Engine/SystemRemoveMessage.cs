using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Messages
{
	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, IReadOnlySystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(IEngine messenger, Type key, IReadOnlySystem value) : base(messenger, key, value)
		{
		}
	}
}
