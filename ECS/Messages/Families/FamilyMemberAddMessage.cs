using Atlas.ECS.Families;

namespace Atlas.Framework.Messages
{
	class FamilyMemberAddMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberAddMessage
	{
		public FamilyMemberAddMessage(IReadOnlyFamily messenger, IFamilyMember value) : base(messenger, value)
		{
		}
	}
}
