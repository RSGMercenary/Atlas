using Atlas.Core.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.ECS.Entities.Messages
{
	public interface IGlobalMatrixMessage : IPropertyMessage<IEntity, Matrix>
	{
	}
}
