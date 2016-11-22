using System;

namespace Atlas.Signals
{
	sealed class Signal:SignalBase, ISignal
	{
		public Signal()
		{

		}

		public void Dispatch()
		{
			if(DispatchStart())
			{
				foreach(Slot slot in Slots)
				{
					try
					{
						slot.Listener.Invoke();
					}
					catch
					{
						//We remove the Slot so the Error doesn't inevitably happen again.
						Remove(slot.Listener);
					}
				}

				DispatchStop();
			}
		}

		public ISlot Get(Action listener)
		{
			return (ISlot)base.Get(listener);
		}

		public new ISlot Get(int index)
		{
			return (ISlot)base.Get(index);
		}

		public int GetIndex(Action listener)
		{
			return base.GetIndex(listener);
		}

		public ISlot Add(Action listener)
		{
			return (ISlot)base.Add(listener);
		}

		public ISlot Add(Action listener, int priority)
		{
			return (ISlot)base.Add(listener, priority);
		}

		public bool Remove(Action listener)
		{
			return base.Remove(listener);
		}

		override protected SlotBase CreateSlot()
		{
			return new Slot();
		}
	}
}