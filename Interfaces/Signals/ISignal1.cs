using System.Collections.Generic;

namespace Atlas.Interfaces.Signals
{
	interface ISignal<T1>:ISignal
	{
		new List<ISlot<T1>> Slots
		{
			get;
		}
	}
}
