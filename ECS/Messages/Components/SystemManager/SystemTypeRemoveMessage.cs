using Atlas.Core.Messages;
using Atlas.ECS.Components;
using System;

namespace Atlas.ECS.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<ISystemManager, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage(ISystemManager messenger, Type value) : base(messenger, value)
		{
		}
	}
}
