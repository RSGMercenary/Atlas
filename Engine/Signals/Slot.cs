using System;

namespace Atlas.Engine.Signals
{
	class SlotBase:ISlotBase
	{
		protected SignalBase signal;
		protected Delegate listener;
		private int priority = 0;

		public SlotBase()
		{

		}

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

	class Slot:SlotBase, ISlot
	{
		public Slot()
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

	class Slot<T1>:SlotBase, ISlot<T1>
	{
		public Slot()
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

	class Slot<T1, T2>:SlotBase, ISlot<T1, T2>
	{
		public Slot()
		{

		}

		public new ISignal<T1, T2> Signal
		{
			get
			{
				return (ISignal<T1, T2>)signal;
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

	class Slot<T1, T2, T3>:SlotBase, ISlot<T1, T2, T3>
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

	class Slot<T1, T2, T3, T4>:SlotBase, ISlot<T1, T2, T3, T4>
	{
		public Slot()
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