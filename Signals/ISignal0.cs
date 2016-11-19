using System;
using System.Collections.Generic;

namespace Atlas.Signals
{
	interface ISignal:ISignalBase
	{
		new List<ISlot> Slots { get; }

		void Dispatch();

		ISlot Get(Action listener);
		new ISlot Get(int index);
		int GetIndex(Action listener);

		ISlot Add(Action listener);
		ISlot Add(Action listener, int priority = 0);

		bool Remove(Action listener);

	}
}
