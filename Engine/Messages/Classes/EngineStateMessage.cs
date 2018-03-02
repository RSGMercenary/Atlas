namespace Atlas.Engine.Messages
{
	public class EngineStateMessage : PropertyMessage<IEngineObject, EngineObjectState>, IEngineStateMessage
	{
		public EngineStateMessage(EngineObjectState current, EngineObjectState previous) : base(current, previous)
		{
		}
	}
}
