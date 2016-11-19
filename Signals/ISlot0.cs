using System;

namespace Atlas.Signals
{
	interface ISlot:ISlotBase
	{
		new ISignal Signal { get; }
		new Action Listener { get; }
	}
}
