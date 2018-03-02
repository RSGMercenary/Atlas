using Atlas.Engine.Entities;

namespace Atlas.Engine.Messages
{
	public class GlobalNameMessage : PropertyMessage<IEntity, string>, IGlobalNameMessage
	{
		public GlobalNameMessage(string current, string previous) : base(current, previous)
		{
		}
	}
}
