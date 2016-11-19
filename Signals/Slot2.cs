using System;

namespace Atlas.Signals
{
	sealed class Slot<T1, T2>:SlotBase
	{
		internal Slot()
		{

		}

		public new Signal<T1, T2> Signal
		{
			get
			{
				return (Signal<T1, T2>)signal;
			}
		}

		public new Action<T1, T2> Listener
		{
			get
			{
				return (Action<T1, T2>)listener;
			}
		}
	}
}