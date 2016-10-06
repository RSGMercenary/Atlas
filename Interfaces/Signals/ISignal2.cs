using System.Collections.Generic;

namespace Atlas.Interfaces.Signals
{
	interface ISignal<T1, T2>:ISignal
	{
		new List<ISlot<T1, T2>> Slots
		{
			get;
		}
	}
}
