using System;

namespace Atlas.Signals
{
	sealed class Signal<T1>:SignalBase
	{
		public Signal()
		{

		}

		public void Dispatch(T1 item1)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1> slot in Slots)
				{
					try
					{
						slot.Listener.Invoke(item1);
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

		public Slot<T1> Get(Action<T1> listener)
		{
			return (Slot<T1>)base.Get(listener);
		}

		public new Slot<T1> GetAt(int index)
		{
			return (Slot<T1>)base.GetAt(index);
		}

		public int GetIndex(Action<T1> listener)
		{
			return base.GetIndex(listener);
		}

		public Slot<T1> Add(Action<T1> listener)
		{
			return Add(listener, 0);
		}

		public Slot<T1> Add(Action<T1> listener, int priority)
		{
			return (Slot<T1>)base.Add(listener, priority);
		}

		public bool Remove(Action<T1> listener)
		{
			return base.Remove(listener);
		}

		override protected SlotBase CreateSlot()
		{
			return new Slot<T1>();
		}
	}
}