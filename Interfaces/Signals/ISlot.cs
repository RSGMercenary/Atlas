using System;

namespace Atlas.Interfaces.Signals
{
	interface ISlot
	{
		ISignal Signal
		{
			get;
		}

		Delegate Listener
		{
			get;
		}

		int Priority
		{
			get;
			set;
		}
	}
}
