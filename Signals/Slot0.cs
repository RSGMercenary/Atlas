using System;

namespace Atlas.Signals
{
	sealed class Slot:SlotBase, ISlot
	{
		internal Slot()
		{

		}

		public new ISignal Signal
		{
			get
			{
				return (ISignal)signal;
			}
		}

		public new Action Listener
		{
			get
			{
				return (Action)listener;
			}
		}
	}
}