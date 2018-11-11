using Atlas.Core.Messages;
using Atlas.ECS.Components;
using System;

namespace Atlas.ECS.Messages
{
	class SystemTypeAddMessage : ValueMessage<ISystemManager, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(ISystemManager messenger, Type value) : base(messenger, value)
		{
		}
	}
}
