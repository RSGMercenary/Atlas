namespace Atlas.Messages
{
	interface IMessage<TSender>
	{
		TSender First { get; }
		TSender Current { get; }
		MessageType MessageType { get; }
	}
}
