﻿using System;

namespace Atlas.Engine.Signals
{
	class Signal<T1, T2>:SignalBase, ISignal<T1, T2>
	{
		public Signal()
		{

		}

		public bool Dispatch(T1 item1, T2 item2)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1, T2> slot in SlotsCopy)
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
				return true;
			}
			return false;
		}

		public ISlot<T1, T2> Get(Action<T1, T2> listener)
		{
			return (ISlot<T1, T2>)base.Get(listener);
		}

		public new ISlot<T1, T2> Get(int index)
		{
			return (ISlot<T1, T2>)base.Get(index);
		}

		public int GetIndex(Action<T1, T2> listener)
		{
			return base.GetIndex(listener);
		}

		public ISlot<T1, T2> Add(Action<T1, T2> listener)
		{
			return (ISlot<T1, T2>)base.Add(listener);
		}

		public ISlot<T1, T2> Add(Action<T1, T2> listener, int priority = 0)
		{
			return (ISlot<T1, T2>)base.Add(listener, priority);
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