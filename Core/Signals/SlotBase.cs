using System;

namespace Atlas.Core.Signals
{
	public class SlotBase : ISlotBase
	{
		private int priority = 0;
		public ISignalBase Signal { get; internal set; }
		public Delegate Listener { get; internal set; }
		public bool IsRemoved { get; internal set; } = false;

		public void Dispose()
		{
			if(Signal != null)
			{
				Signal.Remove(Listener);
			}
			else
			{
				Signal = null;
				Listener = null;
				priority = 0;
			}
		}

		public int Priority
		{
			get => priority;
			set
			{
				if(priority == value)
					return;
				priority = value;
				(Signal as SignalBase)?.Prioritize(this);
			}
		}
	}

	public class SlotBase<TSignal, TDelegate> : SlotBase
		where TSignal : ISignalBase
		where TDelegate : Delegate
	{
		public new TSignal Signal
		{
			get => (TSignal)base.Signal;
			set => base.Signal = value;
		}

		public new TDelegate Listener
		{
			get => (TDelegate)base.Listener;
			set => base.Listener = value;
		}
	}
}
