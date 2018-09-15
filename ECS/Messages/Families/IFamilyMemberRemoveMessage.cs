using Atlas.Core.Messages;
using Atlas.ECS.Families;

namespace Atlas.ECS.Messages
{
	public interface IFamilyMemberRemoveMessage : IValueMessage<IReadOnlyFamily, IFamilyMember>
	{

	}
}
