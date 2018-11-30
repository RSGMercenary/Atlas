using Atlas.Core.Messages;

namespace Atlas.ECS.Families.Messages
{
	class FamilyMemberAddMessage<TFamilyMember> : ValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberAddMessage<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		public FamilyMemberAddMessage(IReadOnlyFamily<TFamilyMember> messenger, TFamilyMember value) : base(messenger, value)
		{
		}
	}
}
