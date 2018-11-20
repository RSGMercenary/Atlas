using Atlas.Core.Messages;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemAddMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemAddMessage
	{
		public SystemAddMessage(IEngine messenger, Type key, ISystem value) : base(messenger, key, value)
		{
		}
	}
}
