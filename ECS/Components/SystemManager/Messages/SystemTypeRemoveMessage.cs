using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<ISystemManager, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage(ISystemManager messenger, Type value) : base(messenger, value)
		{
		}
	}
}
