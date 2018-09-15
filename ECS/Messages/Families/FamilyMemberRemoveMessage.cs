using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	class FamilyMemberRemoveMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberRemoveMessage
	{
		public FamilyMemberRemoveMessage(IReadOnlyFamily messenger, IFamilyMember value) : base(messenger, value)
		{
		}
	}
}
