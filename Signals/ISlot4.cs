using System;

namespace Atlas.Signals
{
	interface ISlot<T1, T2, T3, T4>:ISlotBase
	{
		new ISignal<T1, T2, T3, T4> Signal { get; }
		new Action<T1, T2, T3, T4> Listener { get; }
	}
}
