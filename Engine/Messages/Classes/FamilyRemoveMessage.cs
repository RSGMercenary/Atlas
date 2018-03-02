using Atlas.Engine.Components;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Messages
{
	class FamilyRemoveMessage : KeyValueMessage<IEngine, Type, IFamily>, IFamilyRemoveMessage
	{
		public FamilyRemoveMessage(Type key, IFamily value) : base(key, value)
		{
		}
	}
}
