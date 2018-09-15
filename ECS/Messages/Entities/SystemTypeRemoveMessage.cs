using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<IEntity, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage(IEntity messenger, Type value) : base(messenger, value)
		{
		}
	}
}
