using System;

namespace Atlas.Signals
{
	sealed class Slot<T1, T2, T3>:SlotBase, ISlot<T1, T2, T3>
	{
		internal Slot()
		{

		}

		public new ISignal<T1, T2, T3> Signal
		{
			get
			{
				return (ISignal<T1, T2, T3>)signal;
			}
		}

		public new Action<T1, T2, T3> Listener
		{
			get
			{
				return (Action<T1, T2, T3>)listener;
			}
		}
	}
}