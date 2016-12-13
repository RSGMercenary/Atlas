using System;

namespace Atlas.Testing.Messages
{
	interface IMessage:IDisposable
	{
		string Type { get; }
		object Sender { get; }
		void Initialize(string type, object source, object sender);
	}

	interface IMessage<TDispatcher>:IMessage
	{
		new TDispatcher Sender { get; }
		void Initialize(string type, TDispatcher source, TDispatcher sender);
	}

	interface IMessage<TDispatcher, T1>:IMessage<TDispatcher>
	{
		void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1);
	}

	interface IMessage<TDispatcher, T1, T2>:IMessage<TDispatcher, T1>
	{
		void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2);
	}

	interface IMessage<TDispatcher, T1, T2, T3>:IMessage<TDispatcher, T1, T2>
	{
		void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2, T3 item3);
	}

	interface IMessage<TDispatcher, T1, T2, T3, T4>:IMessage<TDispatcher, T1, T2, T3>
	{
		void Initialize(string type, TDispatcher source, TDispatcher sender, T1 item1, T2 item2, T3 item3, T4 item4);
	}
}
