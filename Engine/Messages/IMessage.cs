namespace Atlas.Engine.Messages
{
	public interface IMessage<TSender> : IMessageBase<TSender>
	{
		/// <summary>
		/// The TSender that first created and sent the Message.
		/// </summary>
		new TSender Target { get; }

		/// <summary>
		/// The TSender that is currently processing the Message.
		/// </summary>
		new TSender CurrentTarget { get; }
	}
}
