using System;

namespace Atlas.Signals
{
	interface ISlotBase
	{
		ISignalBase Signal { get; }
		Delegate Listener { get; }
		int Priority { get; set; }
	}
}
