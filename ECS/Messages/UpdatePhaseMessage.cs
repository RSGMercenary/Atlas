using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	class UpdatePhaseMessage : PropertyMessage<IUpdatePhaseObject, UpdatePhase>, IUpdatePhaseMessage
	{
		public UpdatePhaseMessage(UpdatePhase current, UpdatePhase previous) : base(current, previous)
		{
		}
	}
}
