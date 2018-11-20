using Atlas.Core.Messages;
using Atlas.ECS.Families;
using System;

namespace Atlas.ECS.Components.Messages
{
	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(IEngine messenger, Type key, IReadOnlyFamily value) : base(messenger, key, value)
		{
		}
	}
}
