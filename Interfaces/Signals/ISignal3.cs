using System.Collections.Generic;

namespace Atlas.Interfaces.Signals
{
	interface ISignal<T1, T2, T3>:ISignal
	{
		new List<ISlot<T1, T2, T3>> Slots
		{
			get;
		}
	}
}
