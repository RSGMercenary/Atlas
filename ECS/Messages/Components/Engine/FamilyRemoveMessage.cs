using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Framework.Messages
{
	class FamilyRemoveMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyRemoveMessage
	{
		public FamilyRemoveMessage(Type key, IReadOnlyFamily value) : base(key, value)
		{
		}
	}
}
