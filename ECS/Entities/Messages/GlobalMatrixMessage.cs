using Atlas.Core.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.ECS.Entities.Messages
{
	class GlobalMatrixMessage : PropertyMessage<IEntity, Matrix>, IGlobalMatrixMessage
	{
		public GlobalMatrixMessage(IEntity messenger, Matrix current, Matrix previous) : base(messenger, current, previous)
		{
		}
	}
}
