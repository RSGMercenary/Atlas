using Atlas.ECS.Entities;
using System;

namespace Atlas.Core.Messages
{
	class SystemTypeAddMessage : ValueMessage<IEntity, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(IEntity messenger, Type value) : base(messenger, value)
		{
		}
	}
}
