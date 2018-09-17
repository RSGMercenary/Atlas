using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	class FamilyMemberRemoveMessage<TFamilyMember> : ValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberRemoveMessage<TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{
		public FamilyMemberRemoveMessage(IReadOnlyFamily<TFamilyMember> messenger, TFamilyMember value) : base(messenger, value)
		{
		}
	}
}
