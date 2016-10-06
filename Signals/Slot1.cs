using System;

namespace Atlas.Signals
{
	sealed class Slot<T1>:Slot
	{
		internal Slot()
		{

		}

		public new Signal<T1> Signal
		{
			get
			{
				return (Signal<T1>)signal;
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