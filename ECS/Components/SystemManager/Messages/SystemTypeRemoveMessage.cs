using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<ISystemManager, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage(Type value) : base(value)
		{
		}
	}
}
