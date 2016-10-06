using System;
using System.Collections.Generic;

namespace Atlas.Interfaces.Signals
{
	interface ISignal
	{
		List<ISlot> Slots
		{
			get;
		}

		int SlotCount
		{
			get;
		}

		int DispatchCount
		{
			get;
		}

		void Dispatch(params object[] items);

		bool Has(Delegate listener);

		ISlot Get(Delegate listener);
		ISlot Get(int index);
		int GetIndex(Delegate listener);

		ISlot Add(Delegate listener);
		ISlot Add(Delegate listener, int priority);

		bool Remove(Delegate listener);
		bool Remove(int index);
		void RemoveAll();
	}
}
