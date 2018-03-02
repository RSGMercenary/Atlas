namespace Atlas.Engine.Messages
{
	public interface IMessageBase
	{
		object Target { get; set; }
		object CurrentTarget { get; set; }
		bool AtTarget { get; }
	}

	public interface IMessageBase<TSender> : IMessageBase
	{
		new TSender Target { get; set; }
	}
}
