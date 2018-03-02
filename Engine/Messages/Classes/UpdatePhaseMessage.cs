using Atlas.Engine.Systems;

namespace Atlas.Engine.Messages
{
	class UpdatePhaseMessage : PropertyMessage<IEngineUpdate, UpdatePhase>, IUpdatePhaseMessage
	{
		public UpdatePhaseMessage(UpdatePhase current, UpdatePhase previous) : base(current, previous)
		{
		}
	}
}
