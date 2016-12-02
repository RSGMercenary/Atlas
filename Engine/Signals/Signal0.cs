using Atlas.Engine.Interfaces;
using System;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	class Signal:SignalBase, ISignal, IDispatch
	{
		public Signal()
		{

		}

		public bool Dispatch()
		{
			if(DispatchStart())
			{
				foreach(Slot slot in SlotsCopy)
				{
					try
					{
						slot.Listener.Invoke();
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