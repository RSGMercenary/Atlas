using Atlas.Engine.Interfaces;
using System;

namespace Atlas.Engine.Signals
{
	class Signal<T1, T2, T3, T4>:SignalBase, ISignal<T1, T2, T3, T4>, IDispatch<T1, T2, T3, T4>
	{
		public Signal()
		{

		}

		public bool Dispatch(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1, T2, T3, T4> slot in SlotsCopy)
				{
					try
					{
						slot.Listener.Invoke(item1, item2, item3, item4);
					}
					catch
					{
						//We remove the Slot so the Error doesn't inevitably happen again.
						Remove(slot.Listener);
					}
				}
				DispatchStop();
				return true;
			}
			return false;
		}

		public ISlot<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> listener)
		{
			return (ISlot<T1, T2, T3, T4>)base.Get(listener);
		}

		public new ISlot<T1, T2, T3, T4> Get(int index)
		{
			return (ISlot<T1, T2, T3, T4>)base.Get(index);
		}

		public int GetIndex(Action<T1, T2, T3, T4> listener)
		{
			return base.GetIndex(listener);
		}

		public ISlot<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> listener)
		{
			return (ISlot<T1, T2, T3, T4>)base.Add(listener);
		}

		public ISlot<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> listener, int priority = 0)
		{
			return (ISlot<T1, T2, T3, T4>)base.Add(listener, priority);
		}

		public bool Remove(Action<T1, T2, T3, T4> listener)
		{
			return base.Remove(listener);
		}

		override protected SlotBase CreateSlot()
		{
			return new Slot<T1, T2, T3, T4>();
		}
	}
}