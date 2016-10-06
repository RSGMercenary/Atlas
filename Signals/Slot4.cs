using System;

namespace Atlas.Signals
{
	sealed class Slot<T1, T2, T3, T4>:Slot
	{
		internal Slot()
		{

		}

		public new Signal<T1, T2, T3, T4> Signal
		{
			get
			{
				return (Signal<T1, T2, T3, T4>)signal;
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