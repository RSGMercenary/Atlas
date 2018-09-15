using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	public interface IParentMessage : IPropertyMessage<IEntity, IEntity>
	{
	}
}
