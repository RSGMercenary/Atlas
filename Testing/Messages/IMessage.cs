using System;

namespace Atlas.Testing.Messages
{
	interface IMessage:IDisposable
	{
		void Initialize();
	}

	interface IMessage<T1>:IMessage
	{
		void Initialize(T1 item1);
	}

	interface IMessage<T1, T2>:IMessage<T1>
	{
		void Initialize(T1 item1, T2 item2);
	}

	interface IMessage<T1, T2, T3>:IMessage<T1, T2>
	{
		void Initialize(T1 item1, T2 item2, T3 item3);
	}

	interface IMessage<T1, T2, T3, T4>:IMessage<T1, T2, T3>
	{
		void Initialize(T1 item1, T2 item2, T3 item3, T4 item4);
	}

	interface IMessage<T1, T2, T3, T4, T5>:IMessage<T1, T2, T3, T4>
	{
		void Initialize(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5);
	}

	interface IMessage<T1, T2, T3, T4, T5, T6>:IMessage<T1, T2, T3, T4, T5>
	{
		void Initialize(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6);
	}
}
