using Atlas.Core.Messages;

namespace Atlas.ECS.Entities.Messages
{
	public interface IHierarchyMessage : IMessage<IEntity>
	{
		int Value { get; }
	}
}
