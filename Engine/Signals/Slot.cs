using System;

namespace Atlas.Engine.Signals
{
	class SlotBase : ISlotBase
	{
		public static implicit operator bool(SlotBase slot)
		{
			return slot != null;
		}

		private SignalBase signal;
		private Delegate listener;
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
				signal = value as SignalBase;
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

	class SlotBase<TSignal, TDelegate> : SlotBase
		where TSignal : class, ISignalBase
		where TDelegate : class
	{
		public new TSignal Signal
		{
			get
			{
				return base.Signal as TSignal;
			}
			set
			{
				base.Signal = value as SignalBase;
			}
		}

		public new TDelegate Listener
		{
			get
			{
				return base.Listener as TDelegate;
			}
			set
			{
				base.Listener = value as Delegate;
			}
		}
	}

	class Slot : SlotBase<ISignal, Action>, ISlot
	{

	}

	class Slot<T1> : SlotBase<ISignal<T1>, Action<T1>>, ISlot<T1>
	{

	}

	class Slot<T1, T2> : SlotBase<ISignal<T1, T2>, Action<T1, T2>>, ISlot<T1, T2>
	{

	}

	class Slot<T1, T2, T3> : SlotBase<ISignal<T1, T2, T3>, Action<T1, T2, T3>>, ISlot<T1, T2, T3>
	{

	}

	class Slot<T1, T2, T3, T4> : SlotBase<ISignal<T1, T2, T3, T4>, Action<T1, T2, T3, T4>>, ISlot<T1, T2, T3, T4>
	{

	}
}