using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Messages
{
	class SystemTypeAddMessage : ValueMessage<IEntity, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(IEntity messenger, Type value) : base(messenger, value)
		{
		}
	}
}
