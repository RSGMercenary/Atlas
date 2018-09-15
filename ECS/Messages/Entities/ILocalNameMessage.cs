using Atlas.Core.Messages;
using Atlas.ECS.Entities;

namespace Atlas.ECS.Messages
{
	public interface ILocalNameMessage : IPropertyMessage<IEntity, string>
	{
	}
}
