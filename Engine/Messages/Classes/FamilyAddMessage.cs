using Atlas.Engine.Components;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Messages
{
	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(Type key, IFamily value) : base(key, value)
		{
		}
	}
}
