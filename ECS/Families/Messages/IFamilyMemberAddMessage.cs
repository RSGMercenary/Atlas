using Atlas.Core.Messages;

namespace Atlas.ECS.Families.Messages
{
	public interface IFamilyMemberAddMessage<TFamilyMember> : IValueMessage<IFamily<TFamilyMember>, TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
	}
}