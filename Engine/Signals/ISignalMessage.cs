using Atlas.Engine.Interfaces;
using Atlas.Testing.Messages;

namespace Atlas.Engine.Signals
{
	interface ISignalMessage<TMessage, TSender>:ISignal<TMessage>, IDispatch<string, TSender> where TMessage : IMessage<TSender>
	{

	}

	interface ISignalMessage<TMessage, TSender, T1>:ISignal<TMessage>, IDispatch<string, TSender, T1> where TMessage : IMessage<TSender, T1>, new()
	{

	}

	interface ISignalMessage<TMessage, TSender, T1, T2>:ISignal<TMessage>, IDispatch<string, TSender, T1, T2> where TMessage : IMessage<TSender, T1, T2>, new()
	{

	}

	interface ISignalMessage<TMessage, TSender, T1, T2, T3>:ISignal<TMessage>, IDispatch<string, TSender, T1, T2, T3> where TMessage : IMessage<TSender, T1, T2, T3>, new()
	{

	}

	interface ISignalMessage<TMessage, TSender, T1, T2, T3, T4>:ISignal<TMessage>, IDispatch<string, TSender, T1, T2, T3, T4> where TMessage : IMessage<TSender, T1, T2, T3, T4>, new()
	{

	}
}
