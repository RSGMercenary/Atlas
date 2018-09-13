using Atlas.ECS.Families;

namespace Atlas.Framework.Messages
{
	class FamilyMemberRemoveMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberRemoveMessage
	{
		public FamilyMemberRemoveMessage(IReadOnlyFamily messenger, IFamilyMember value) : base(messenger, value)
		{
		}
	}
}
