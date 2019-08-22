using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Messages
{
	class BuildStateMessage : PropertyMessage<IBuilder, BuildState>, IBuildStateMessage
	{
		public BuildStateMessage(BuildState current, BuildState previous) : base(current, previous)
		{

		}
	}
}
