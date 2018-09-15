using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	class FamilyMemberAddMessage : ValueMessage<IReadOnlyFamily, IFamilyMember>, IFamilyMemberAddMessage
	{
		public FamilyMemberAddMessage(IReadOnlyFamily messenger, IFamilyMember value) : base(messenger, value)
		{
		}
	}
}
