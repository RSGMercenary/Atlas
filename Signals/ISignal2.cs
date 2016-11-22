using System;

namespace Atlas.Signals
{
	interface ISignal<T1, T2>:ISignalBase
	{
		//new List<ISlot<T1, T2>> Slots { get; }

		void Dispatch(T1 item1, T2 item2);

		ISlot<T1, T2> Get(Action<T1, T2> listener);
		new ISlot<T1, T2> Get(int index);
		int GetIndex(Action<T1, T2> listener);

		ISlot<T1, T2> Add(Action<T1, T2> listener);
		ISlot<T1, T2> Add(Action<T1, T2> listener, int priority = 0);

		bool Remove(Action<T1, T2> listener);

	}
}
