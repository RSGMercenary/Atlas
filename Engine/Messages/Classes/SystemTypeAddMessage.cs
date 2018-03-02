using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Messages
{
	class SystemTypeAddMessage : ValueMessage<IEntity, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(Type value) : base(value)
		{
		}
	}
}
