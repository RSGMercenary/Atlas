using Atlas.Core.Messages;

namespace Atlas.ECS.Families.Messages
{
	class FamilyMemberAddMessage<TFamilyMember> : ValueMessage<IFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberAddMessage<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		public FamilyMemberAddMessage(IFamily<TFamilyMember> messenger, TFamilyMember value) : base(messenger, value)
		{
		}
	}
}
