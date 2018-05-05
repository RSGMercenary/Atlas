using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.Framework.Messages
{
	class SystemRemoveMessage : KeyValueMessage<IEngine, Type, IReadOnlySystem>, ISystemRemoveMessage
	{
		public SystemRemoveMessage(Type key, IReadOnlySystem value) : base(key, value)
		{
		}
	}
}
