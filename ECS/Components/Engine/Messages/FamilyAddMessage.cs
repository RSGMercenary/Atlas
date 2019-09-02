using Atlas.Core.Messages;
using Atlas.ECS.Families;
using System;

namespace Atlas.ECS.Components.Messages
{
	class FamilyAddMessage : KeyValueMessage<IEngine, Type, IFamily>, IFamilyAddMessage
	{
		public FamilyAddMessage(Type key, IFamily value) : base(key, value) { }
	}
}