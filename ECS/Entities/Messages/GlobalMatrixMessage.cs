using Atlas.Core.Messages;
using Microsoft.Xna.Framework;

namespace Atlas.ECS.Entities.Messages
{
	class GlobalMatrixMessage : PropertyMessage<IEntity, Matrix>, IGlobalMatrixMessage
	{
		public bool Hierarchy { get; }

		public GlobalMatrixMessage(IEntity messenger, Matrix current, Matrix previous, bool hierarchy) : base(messenger, current, previous)
		{
			Hierarchy = hierarchy;
		}
	}
}
