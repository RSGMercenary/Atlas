using Atlas.ECS.Entities;
using System;

namespace Atlas.Framework.Messages
{
	class SystemTypeAddMessage : ValueMessage<IEntity, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(Type value) : base(value)
		{
		}
	}
}
