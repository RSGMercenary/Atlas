using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Messages
{
	class SystemTypeRemoveMessage : ValueMessage<IEntity, Type>, ISystemTypeRemoveMessage
	{
		public SystemTypeRemoveMessage(Type value) : base(value)
		{
		}
	}
}
