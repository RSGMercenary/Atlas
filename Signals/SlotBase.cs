using System;

namespace Atlas.Signals
{
	class SlotBase:ISlotBase
	{
		protected SignalBase signal;
		protected Delegate listener;
		private int priority = 0;

		public void Dispose()
		{
			if(signal != null)
			{
				signal.Remove(listener);
			}
			else
			{
				signal = null;
				listener = null;
				priority = 0;
			}
		}

		public ISignalBase Signal
		{
			get
			{
				return signal;
			}
			set
			{
				signal = (SignalBase)value;
			}
		}

		public Delegate Listener
		{
			get
			{
				return listener;
			}
			set
			{
				listener = value;
			}
		}

		public int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				if(priority != value)
				{
					int previous = priority;
					priority = value;
					if(signal != null)
					{
						signal.PriorityChanged(this, value, previous);
					}
				}
			}
		}
	}
}