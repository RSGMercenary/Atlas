using System;

namespace Atlas.Signals
{
	sealed class Slot<T1, T2, T3, T4>:SlotBase, ISlot<T1, T2, T3, T4>
	{
		internal Slot()
		{

		}

		public new ISignal<T1, T2, T3, T4> Signal
		{
			get
			{
				return (ISignal<T1, T2, T3, T4>)signal;
			}
		}

		public new Action<T1, T2, T3, T4> Listener
		{
			get
			{
				return (Action<T1, T2, T3, T4>)listener;
			}
		}
	}
}