using Atlas.Core.Messages;
using Atlas.ECS.Families;
using System;

namespace Atlas.ECS.Components.Messages
{
	class FamilyRemoveMessage : KeyValueMessage<IEngine, Type, IFamily>, IFamilyRemoveMessage
	{
		public FamilyRemoveMessage(IEngine messenger, Type key, IFamily value) : base(messenger, key, value)
		{
		}
	}
}
