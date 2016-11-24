namespace Atlas.Messages
{
	interface IMessage
	{
		string Type { get; }
		object Sender { get; }
	}

	interface IMessage<TSender>:IMessage
	{
		new TSender Sender { get; }
	}

	interface IMessage<TSender, T1>:IMessage<TSender>
	{

	}

	interface IMessage<TSender, T1, T2>:IMessage<TSender, T1>
	{

	}

	interface IMessage<TSender, T1, T2, T3>:IMessage<TSender, T1, T2>
	{

	}

	interface IMessage<TSender, T1, T2, T3, T4>:IMessage<TSender, T1, T2, T3>
	{

	}
}
