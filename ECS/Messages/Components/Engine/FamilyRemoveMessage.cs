using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.ECS.Messages
{
	class FamilyRemoveMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyRemoveMessage
	{
		public FamilyRemoveMessage(IEngine messenger, Type key, IReadOnlyFamily value) : base(messenger, key, value)
		{
		}
	}
}
