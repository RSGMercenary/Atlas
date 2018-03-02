namespace Atlas.Engine.Messages
{
	public interface IMessage : IMessageBase
	{
		new object Target { get; }
		new object CurrentTarget { get; }
	}

	public interface IMessage<TSender> : IMessage, IMessageBase<TSender>
	{
		/// <summary>
		/// The TSender that first created and sent the Message.
		/// </summary>
		new TSender Target { get; }

		/// <summary>
		/// The IMessageDispatcher that is currently processing the Message.
		/// </summary>
		new object CurrentTarget { get; }
	}
}
