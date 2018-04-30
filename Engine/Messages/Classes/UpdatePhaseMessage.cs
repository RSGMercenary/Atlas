using Atlas.Engine.Engine;

namespace Atlas.Engine.Messages
{
	class UpdatePhaseMessage : PropertyMessage<IUpdatePhaseEngineObject, UpdatePhase>, IUpdatePhaseMessage
	{
		public UpdatePhaseMessage(UpdatePhase current, UpdatePhase previous) : base(current, previous)
		{
		}
	}
}
