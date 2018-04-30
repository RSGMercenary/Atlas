using Atlas.Engine.Entities;
using Atlas.Engine.Families;

namespace Atlas.Engine.Messages
{
	class FamilyEntityRemoveMessage : ValueMessage<IFamily, IEntity>, IFamilyEntityRemoveMessage
	{
		public FamilyEntityRemoveMessage(IEntity value) : base(value)
		{
		}
	}
}
