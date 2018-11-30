using Atlas.Core.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.ECS.Entities.Messages
{
	class LocalMatrixMessage : PropertyMessage<IEntity, Matrix>, ILocalMatrixMessage
	{
		public LocalMatrixMessage(IEntity messenger, Matrix current, Matrix previous) : base(messenger, current, previous)
		{
		}
	}
}
