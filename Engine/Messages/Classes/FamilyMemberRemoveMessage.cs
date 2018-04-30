using Atlas.Engine.Families;

namespace Atlas.Engine.Messages
{
	class FamilyMemberRemoveMessage : ValueMessage<IFamily, IFamilyMember>, IFamilyMemberRemoveMessage
	{
		public FamilyMemberRemoveMessage(IFamilyMember value) : base(value)
		{
		}
	}
}
