using Atlas.Core.Messages;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemAddMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemAddMessage
	{
		public SystemAddMessage(Type key, ISystem value) : base(key, value)
		{
		}
	}
}
