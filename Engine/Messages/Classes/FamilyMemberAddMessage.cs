using Atlas.Engine.Families;

namespace Atlas.Engine.Messages
{
	class FamilyMemberAddMessage : ValueMessage<IFamily, IFamilyMember>, IFamilyEntityAddMessage
	{
		public FamilyMemberAddMessage(IFamilyMember value) : base(value)
		{
		}
	}
}
