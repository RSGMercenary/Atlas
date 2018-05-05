using Atlas.ECS.Components;
using Atlas.ECS.Systems;
using System;

namespace Atlas.Framework.Messages
{
	class SystemAddMessage : KeyValueMessage<IEngine, Type, IReadOnlySystem>, ISystemAddMessage
	{
		public SystemAddMessage(Type key, IReadOnlySystem value) : base(key, value)
		{
		}
	}
}
