using System;

namespace Atlas.Signals
{
	interface ISignal<T1, T2, T3>:ISignalBase
	{
		//new List<ISlot<T1, T2, T3>> Slots { get; }

		void Dispatch(T1 item1, T2 item2, T3 item3);

		ISlot<T1, T2, T3> Get(Action<T1, T2, T3> listener);
		new ISlot<T1, T2, T3> Get(int index);
		int GetIndex(Action<T1, T2, T3> listener);

		ISlot<T1, T2, T3> Add(Action<T1, T2, T3> listener);
		ISlot<T1, T2, T3> Add(Action<T1, T2, T3> listener, int priority = 0);

		bool Remove(Action<T1, T2, T3> listener);

	}
}
