using System;

namespace Atlas.Engine.Signals
{
	interface ISlotBase:IDisposable
	{
		ISignalBase Signal { get; }
		Delegate Listener { get; }
		int Priority { get; set; }
	}

	interface ISlotBase<TSignal, TDelegate>:ISlotBase
		where TSignal : class, ISignalBase
		where TDelegate : class
	{
		new TSignal Signal { get; }
		new TDelegate Listener { get; }
	}

	interface ISlot:ISlotBase<ISignal, Action>
	{

	}

	interface ISlot<T1>:ISlotBase<ISignal<T1>, Action<T1>>
	{

	}

	interface ISlot<T1, T2>:ISlotBase<ISignal<T1, T2>, Action<T1, T2>>
	{

	}

	interface ISlot<T1, T2, T3>:ISlotBase<ISignal<T1, T2, T3>, Action<T1, T2, T3>>
	{

	}

	interface ISlot<T1, T2, T3, T4>:ISlotBase<ISignal<T1, T2, T3, T4>, Action<T1, T2, T3, T4>>
	{

	}
}
