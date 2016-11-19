using System;

namespace Atlas.Signals
{
	interface ISlot<T1, T2>:ISlotBase
	{
		new ISignal<T1, T2> Signal { get; }
		new Action<T1, T2> Listener { get; }
	}
}
