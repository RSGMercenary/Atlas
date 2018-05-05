using Atlas.ECS.Entities;
using System;

namespace Atlas.Framework.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<IEntity, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage()
		{

		}

		public SystemTypeRemoveMessage(Type value) : base(value)
		{
		}
	}
}
