using Atlas.Core.Messages;

namespace Atlas.ECS.Families.Messages
{
	public interface IFamilyMemberRemoveMessage<TFamilyMember> : IValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{

	}
}
