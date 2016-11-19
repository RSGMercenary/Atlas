using System;
using System.Collections.Generic;

namespace Atlas.Signals
{
	interface ISignal<T1>:ISignalBase
	{
		new List<ISlot<T1>> Slots { get; }

		void Dispatch(T1 item1);

		ISlot<T1> Get(Action<T1> listener);
		new ISlot<T1> Get(int index);
		int GetIndex(Action<T1> listener);

		ISlot<T1> Add(Action<T1> listener);
		ISlot<T1> Add(Action<T1> listener, int priority = 0);

		bool Remove(Action<T1> listener);

	}
}
