using System;
using System.Collections.Generic;

namespace Atlas.Signals
{
	interface ISignal<T1, T2, T3, T4>:ISignalBase
	{
		new List<ISlot<T1, T2, T3, T4>> Slots { get; }

		void Dispatch(T1 item1, T2 item2, T3 item3, T4 item4);

		ISlot<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> listener);
		new ISlot<T1, T2, T3, T4> Get(int index);
		int GetIndex(Action<T1, T2, T3, T4> listener);

		ISlot<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> listener);
		ISlot<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> listener, int priority = 0);

		bool Remove(Action<T1, T2, T3, T4> listener);

	}
}
