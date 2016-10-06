using System;

namespace Atlas.Signals
{
	sealed class Signal<T1, T2>:Signal
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
					if(slot.IsOnce)
					{
						Remove(slot.Listener);
					}

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
			return Add(listener, 0, false);
		}

		public Slot<T1, T2> Add(Action<T1, T2> listener, int priority = 0)
		{
			return Add(listener, priority, false);
		}

		public Slot<T1, T2> AddOnce(Action<T1, T2> listener, int priority = 0)
		{
			return Add(listener, priority, true);
		}

		public Slot<T1, T2> Add(Action<T1, T2> listener, int priority = 0, bool isOnce = false)
		{
			return (Slot<T1, T2>)base.Add(listener, priority, isOnce);
		}

		public bool Remove(Action<T1, T2> listener)
		{
			return base.Remove(listener);
		}

		override protected Slot CreateSlot()
		{
			return new Slot<T1, T2>();
		}
	}
}