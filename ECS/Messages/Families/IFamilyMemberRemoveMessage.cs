using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	public interface IFamilyMemberRemoveMessage<TFamilyMember> : IValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{

	}
}
