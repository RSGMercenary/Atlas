using System;

namespace Atlas.Signals
{
	interface ISlot<T1>:ISlotBase
	{
		new ISignal<T1> Signal { get; }
		new Action<T1> Listener { get; }
	}
}
