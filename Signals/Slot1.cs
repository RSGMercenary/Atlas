using System;

namespace Atlas.Signals
{
	sealed class Slot<T1>:SlotBase, ISlot<T1>
	{
		internal Slot()
		{

		}

		public new ISignal<T1> Signal
		{
			get
			{
				return (ISignal<T1>)signal;
			}
		}

		public new Action<T1> Listener
		{
			get
			{
				return (Action<T1>)listener;
			}
		}
	}
}