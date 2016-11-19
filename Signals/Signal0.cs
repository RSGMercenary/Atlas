using System;

namespace Atlas.Signals
{
	sealed class Signal:SignalBase
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

		public Slot Get(Action listener)
		{
			return (Slot)base.Get(listener);
		}

		public new Slot GetAt(int index)
		{
			return (Slot)base.GetAt(index);
		}

		public int GetIndex(Action listener)
		{
			return base.GetIndex(listener);
		}

		public Slot Add(Action listener)
		{
			return Add(listener, 0);
		}

		public Slot Add(Action listener, int priority)
		{
			return (Slot)base.Add(listener, priority);
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