using System;

namespace Atlas.Testing.Messages
{
	interface IMessage:IDisposable
	{
		string Type { get; }
		object Sender { get; }
		void Initialize(string type, object sender);
	}

	interface IMessage<TSender>:IMessage
	{
		new TSender Sender { get; }
		void Initialize(string type, TSender sender);
	}

	interface IMessage<TSender, T1>:IMessage<TSender>
	{
		void Initialize(string type, TSender sender, T1 item1);
	}

	interface IMessage<TSender, T1, T2>:IMessage<TSender, T1>
	{
		void Initialize(string type, TSender sender, T1 item1, T2 item2);
	}

	interface IMessage<TSender, T1, T2, T3>:IMessage<TSender, T1, T2>
	{
		void Initialize(string type, TSender sender, T1 item1, T2 item2, T3 item3);
	}

	interface IMessage<TSender, T1, T2, T3, T4>:IMessage<TSender, T1, T2, T3>
	{
		void Initialize(string type, TSender sender, T1 item1, T2 item2, T3 item3, T4 item4);
	}
}
