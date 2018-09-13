using Atlas.ECS.Families;

namespace Atlas.Core.Messages
{
	class FamilyMemberAddMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberAddMessage
	{
		public FamilyMemberAddMessage(IReadOnlyFamily messenger, IFamilyMember value) : base(messenger, value)
		{
		}
	}
}
