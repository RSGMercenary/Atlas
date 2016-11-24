using Atlas.Interfaces;
using Atlas.Messages;

namespace Atlas.Engine.Signals
{
	interface ISignalMessage<TMessage>:ISignal<TMessage>, IDispatch where TMessage : IMessage
	{

	}

	interface ISignalMessage<TMessage, T1>:ISignal<TMessage>, IDispatch<T1> where TMessage : IMessage<TSender, T1>
	{

	}

	interface ISignalMessage<TMessage, TSender, T1, T2>:ISignal<TMessage>, IDispatch<TSender, T1, T2> where TMessage : IMessage<TSender, T1, T2>
	{

	}

	interface ISignalMessage<TMessage, TSender, T1, T2, T3>:ISignal<TMessage>, IDispatch<TSender, T1, T2, T3> where TMessage : IMessage<TSender, T1, T2, T3>
	{

	}

	interface ISignalMessage<TMessage, TSender, T1, T2, T3, T4>:ISignal<TMessage>, IDispatch<TSender, T1, T2, T3, T4> where TMessage : IMessage<TSender, T1, T2, T3, T4>
	{

	}
}
