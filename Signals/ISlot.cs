using System;

namespace Atlas.Signals
{
	interface ISlotBase:IDisposable
	{
		ISignalBase Signal { get; }
		Delegate Listener { get; }
		int Priority { get; set; }
	}

	interface ISlot:ISlotBase
	{
		new ISignal Signal { get; }
		new Action Listener { get; }
	}

	interface ISlot<T1>:ISlotBase
	{
		new ISignal<T1> Signal { get; }
		new Action<T1> Listener { get; }
	}

	interface ISlot<T1, T2>:ISlotBase
	{
		new ISignal<T1, T2> Signal { get; }
		new Action<T1, T2> Listener { get; }
	}

	interface ISlot<T1, T2, T3>:ISlotBase
	{
		new ISignal<T1, T2, T3> Signal { get; }
		new Action<T1, T2, T3> Listener { get; }
	}

	interface ISlot<T1, T2, T3, T4>:ISlotBase
	{
		new ISignal<T1, T2, T3, T4> Signal { get; }
		new Action<T1, T2, T3, T4> Listener { get; }
	}
}
