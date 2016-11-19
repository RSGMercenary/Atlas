using System;

namespace Atlas.Signals
{
	sealed class Signal<T1, T2>:SignalBase
	{
		public Signal()
		{

		}

		public void Dispatch(T1 item1, T2 item2)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1, T2> slot in Slots)
				{
					try
					{
						slot.Listener.Invoke(item1, item2);
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

		public Slot<T1, T2> Get(Action<T1, T2> listener)
		{
			return (Slot<T1, T2>)base.Get(listener);
		}

		public new Slot<T1, T2> GetAt(int index)
		{
			return (Slot<T1, T2>)base.GetAt(index);
		}

		public int GetIndex(Action<T1, T2> listener)
		{
			return base.GetIndex(listener);
		}

		public Slot<T1, T2> Add(Action<T1, T2> listener)
		{
			return (Slot<T1, T2>)base.Add(listener);
		}

		public Slot<T1, T2> Add(Action<T1, T2> listener, int priority = 0)
		{
			return (Slot<T1, T2>)base.Add(listener, priority);
		}

		public bool Remove(Action<T1, T2> listener)
		{
			return base.Remove(listener);
		}

		override protected SlotBase CreateSlot()
		{
			return new Slot<T1, T2>();
		}
	}
}