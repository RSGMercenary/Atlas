using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	public interface IFamilyMemberAddMessage<TFamilyMember> : IValueMessage<IReadOnlyFamily<TFamilyMember>, TFamilyMember>
		where TFamilyMember : IFamilyMember, new()
	{

	}
}
