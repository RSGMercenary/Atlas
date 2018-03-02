namespace Atlas.Engine.Messages
{
	class AutoDestroyMessage : PropertyMessage<IAutoEngineObject, bool>, IAutoDestroyMessage
	{
		public AutoDestroyMessage(bool current, bool previous) : base(current, previous)
		{
		}
	}
}
