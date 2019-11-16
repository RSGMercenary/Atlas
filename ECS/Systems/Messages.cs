using Atlas.Core.Messages;

namespace Atlas.ECS.Systems
{
	#region Interfaces

	public interface IIntervalMessage : IPropertyMessage<ISystem, double> { }

	public interface IPriorityMessage : IPropertyMessage<ISystem, int> { }


	#endregion

	#region Classes

	class IntervalMessage : PropertyMessage<ISystem, double>, IIntervalMessage
	{
		public IntervalMessage(double current, double previous) : base(current, previous) { }
	}


	class PriorityMessage : PropertyMessage<ISystem, int>, IPriorityMessage
	{
		public PriorityMessage(int current, int previous) : base(current, previous) { }
	}

	#endregion
}