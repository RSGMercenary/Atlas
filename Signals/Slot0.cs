using System;

namespace Atlas.Signals
{
	sealed class Slot:SlotBase
	{
		internal Slot()
		{

		}

		public new Signal Signal
		{
			get
			{
				return (Signal)signal;
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