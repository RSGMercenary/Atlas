using Atlas.Engine.Interfaces;
using System;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	class Signal<T1>:SignalBase, ISignal<T1>, IDispatch<T1>
	{
		private class Packet<TT1>
		{
			public TT1 item1;
		}

		public Signal()
		{

		}

		virtual public bool Dispatch(T1 item1)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1> slot in SlotsCopy)
				{
					try
					{
						slot.Listener.Invoke(item1);
					}
					catch(Exception e)
					{
						Debug.WriteLine(e);
						//We remove the Slot so the Error doesn't inevitably happen again.
						Remove(slot.Listener);
					}
				}
				DispatchStop();
				return true;
			}
			return false;
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