using Atlas.Engine.Components;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Messages
{
	class SystemAddMessage : KeyValueMessage<IEngine, Type, ISystem>, ISystemAddMessage
	{
		public SystemAddMessage(Type key, ISystem value) : base(key, value)
		{
		}
	}
}
