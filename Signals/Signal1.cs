using System;

namespace Atlas.Signals
{
	sealed class Signal<T1>:Signal
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
					if(slot.IsOnce)
					{
						Remove(slot.Listener);
					}

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
			return Add(listener, 0, false);
		}

		public Slot<T1> Add(Action<T1> listener, int priority)
		{
			return Add(listener, priority, false);
		}

		public Slot<T1> AddOnce(Action<T1> listener, int priority)
		{
			return Add(listener, priority, true);
		}

		public Slot<T1> Add(Action<T1> listener, int priority, bool isOnce)
		{
			return (Slot<T1>)base.Add(listener, priority, isOnce);
		}

		public bool Remove(Action<T1> listener)
		{
			return base.Remove(listener);
		}

		override protected Slot CreateSlot()
		{
			return new Slot<T1>();
		}
	}
}