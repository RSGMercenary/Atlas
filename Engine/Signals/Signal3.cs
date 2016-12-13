using Atlas.Engine.Interfaces;
using System;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	class Signal<T1, T2, T3>:SignalBase<ISlot<T1, T2, T3>, Action<T1, T2, T3>>, ISignal<T1, T2, T3>, IDispatch<T1, T2, T3>
	{
		public Signal()
		{

		}

		public bool Dispatch(T1 item1, T2 item2, T3 item3)
		{
			if(DispatchStart())
			{
				foreach(Slot<T1, T2, T3> slot in SlotsCopy)
				{
					try
					{
						slot.Listener.Invoke(item1, item2, item3);
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

		override protected SlotBase CreateSlot()
		{
			return new Slot<T1, T2, T3>();
		}
	}
}