using Atlas.ECS.Families;

namespace Atlas.Framework.Messages
{
	class FamilyMemberRemoveMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberRemoveMessage
	{
		public FamilyMemberRemoveMessage(IFamilyMember value) : base(value)
		{
		}
	}
}
