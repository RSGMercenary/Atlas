using Atlas.Interfaces;
using System;
using System.Collections.Generic;

namespace Atlas.Signals
{
	interface ISignalBase:IDispose
	{
		int NumDispatches { get; }
		int NumSlots { get; }

		bool HasSlots { get; }

		List<ISlotBase> Slots { get; }

		void Dispatch(params object[] items);

		ISlotBase Get(Delegate listener);
		ISlotBase Get(int index);
		int GetIndex(Delegate listener);

		ISlotBase Add(Delegate listener);
		ISlotBase Add(Delegate listener, int priority = 0);

		bool Remove(Delegate listener);
		bool Remove(int index);
		bool RemoveAll();
	}
}
