using Atlas.ECS.Components;
using Atlas.ECS.Families;
using System;

namespace Atlas.Framework.Messages
{
	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IReadOnlyFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(Type key, IReadOnlyFamily value) : base(key, value)
		{
		}
	}
}
