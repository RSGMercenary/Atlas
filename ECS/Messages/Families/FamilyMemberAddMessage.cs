using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	class FamilyMemberAddMessage<TFamilyMember> : ValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberAddMessage<TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{
		public FamilyMemberAddMessage(IReadOnlyFamily<TFamilyMember> messenger, TFamilyMember value) : base(messenger, value)
		{
		}
	}
}
