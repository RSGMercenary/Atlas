using Atlas.Core.Messages;

namespace Atlas.ECS.Families.Messages
{
	class FamilyMemberRemoveMessage<TFamilyMember> : ValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberRemoveMessage<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		public FamilyMemberRemoveMessage(IReadOnlyFamily<TFamilyMember> messenger, TFamilyMember value) : base(messenger, value)
		{
		}
	}
}
