using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemTypeAddMessage : ValueMessage<ISystemManager, Type>, ISystemTypeAddMessage
	{
		public SystemTypeAddMessage(Type value) : base(value)
		{
		}
	}
}