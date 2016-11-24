using System;

namespace Atlas.Engine.Signals
{
	class Signal<T1>:SignalBase, ISignal<T1>
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

		public ISlot<T1> Get(Action<T1> listener)
		{
			return (ISlot<T1>)base.Get(listener);
		}

		public new ISlot<T1> Get(int index)
		{
			return (ISlot<T1>)base.Get(index);
		}

		public int GetIndex(Action<T1> listener)
		{
			return base.GetIndex(listener);
		}

		public ISlot<T1> Add(Action<T1> listener)
		{
			return (ISlot<T1>)base.Add(listener, 0);
		}

		public ISlot<T1> Add(Action<T1> listener, int priority)
		{
			return (ISlot<T1>)base.Add(listener, priority);
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