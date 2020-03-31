using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Builder
{
	#region Interfaces
	public interface IBuildStateMessage : IPropertyMessage<IBuilder, BuildState> { }
	#endregion

	#region Classes
	class BuildStateMessage : PropertyMessage<IBuilder, BuildState>, IBuildStateMessage
	{
		public BuildStateMessage(BuildState current, BuildState previous) : base(current, previous) { }
	}
	#endregion
}