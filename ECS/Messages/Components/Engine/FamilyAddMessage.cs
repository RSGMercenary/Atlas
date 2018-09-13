using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Framework.Messages
{
	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(IEngine messenger, Type key, IReadOnlyFamily value) : base(messenger, key, value)
		{
		}
	}
}
