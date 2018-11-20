using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemTypeAddMessage : ValueMessage<ISystemManager, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(ISystemManager messenger, Type value) : base(messenger, value)
		{
		}
	}
}
