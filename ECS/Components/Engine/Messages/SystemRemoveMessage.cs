using Atlas.Core.Messages;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Messages
{
	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(Type key, ISystem value) : base(key, value)
		{
		}
	}
}
