namespace Atlas.Messages
{
	interface IMessage<TSender>
	{
		TSender First { get; }
		TSender Current { get; }
		string Type { get; }
	}
}
