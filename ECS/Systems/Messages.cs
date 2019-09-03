using Atlas.Core.Messages;

namespace Atlas.ECS.Systems
{
	#region Interfaces

	public interface IIntervalMessage : IPropertyMessage<ISystem, double> { }

	#endregion

	#region Classes

	class IntervalMessage : PropertyMessage<ISystem, double>, IIntervalMessage
	{
		public IntervalMessage(double current, double previous) : base(current, previous) { }
	}

	#endregion
}