namespace Atlas.Engine.Messages
{
	public interface IMessageBase<TSender>
	{
		/// <summary>
		/// The type of the Message. This Message is sent to anyone
		/// subscribed to this type of Message.
		/// </summary>
		string Type { get; }

		/// <summary>
		/// The TSender that first created and sent the Message.
		/// </summary>
		TSender Target { get; set; }

		/// <summary>
		/// The TSender that is currently processing the Message.
		/// </summary>
		TSender CurrentTarget { get; set; }

		/// <summary>
		/// When the Target and CurrentTarget are equal. This signifies that the
		/// Message is currently happening at the TSender who originally sent the Message.
		/// </summary>
		bool AtTarget { get; }
	}
}
