using Atlas.ECS.Entities;
using System;

namespace Atlas.Core.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<IEntity, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage(IEntity messenger, Type value) : base(messenger, value)
		{
		}
	}
}
