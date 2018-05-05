using Atlas.ECS.Families;

namespace Atlas.Framework.Messages
{
	class FamilyMemberAddMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberAddMessage
	{
		public FamilyMemberAddMessage(IFamilyMember value) : base(value)
		{
		}
	}
}
