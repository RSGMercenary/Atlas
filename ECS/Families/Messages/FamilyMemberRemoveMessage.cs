using Atlas.Core.Messages;

namespace Atlas.ECS.Families.Messages
{
	class FamilyMemberRemoveMessage<TFamilyMember> : ValueMessage<IFamily<TFamilyMember>, TFamilyMember>, IFamilyMemberRemoveMessage<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		public FamilyMemberRemoveMessage(TFamilyMember value) : base(value) { }
	}
}