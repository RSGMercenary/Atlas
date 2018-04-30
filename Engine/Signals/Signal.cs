using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.Engine.Signals
{
	public class SignalDynamic : SignalBase<SlotBase, ISlotBase, Delegate>, ISignalDynamic
	{
		public bool Dispatch(params object[] items)
		{
			if(DispatchStart())
			{
				foreach(SlotBase slot in new List<ISlotBase>(Slots))
				{
					try
					{
						slot.Listener.DynamicInvoke(items);
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
	}

	public class Signal : SignalBase<Slot, ISlot, Action>, ISignal, IDispatch
	{
		public bool Dispatch()
		{
			if(DispatchStart())
			{
				foreach(Slot slot in new List<ISlotBase>(Slots))
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
	}

	public class Signal<T1> : SignalBase<Slot<T1>, ISlot<T1>, Action<T1>>, ISignal<T1>, IDispatch<T1>
	{
		public bool Dispatch(T1 item1)
		{
			if(DispatchStart())
			{
				//Copy the list for each unique dispatch.
				foreach(Slot<T1> slot in new List<ISlotBase>(Slots))
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
	}

	public class Signal<T1, T2> : SignalBase<Slot<T1, T2>, ISlot<T1, T2>, Action<T1, T2>>, ISignal<T1, T2>, IDispatch<T1, T2>
	{
		public bool Dispatch(T1 item1, T2 item2)
		{
			if(DispatchStart())
			{
				//Copy the list for each unique dispatch.
				foreach(Slot<T1, T2> slot in new List<ISlotBase>(Slots))
				{
					try
					{
						slot.Listener.Invoke(item1, item2);
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
	}

	public class Signal<T1, T2, T3> : SignalBase<Slot<T1, T2, T3>, ISlot<T1, T2, T3>, Action<T1, T2, T3>>, ISignal<T1, T2, T3>, IDispatch<T1, T2, T3>
	{
		public bool Dispatch(T1 item1, T2 item2, T3 item3)
		{
			if(DispatchStart())
			{
				//Copy the list for each unique dispatch.
				foreach(Slot<T1, T2, T3> slot in new List<ISlotBase>(Slots))
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
	}

	public class Signal<T1, T2, T3, T4> : SignalBase<Slot<T1, T2, T3, T4>, ISlot<T1, T2, T3, T4>, Action<T1, T2, T3, T4>>, ISignal<T1, T2, T3, T4>, IDispatch<T1, T2, T3, T4>
	{
		public bool Dispatch(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			if(DispatchStart())
			{
				//Copy the list for each unique dispatch.
				foreach(Slot<T1, T2, T3, T4> slot in new List<ISlotBase>(Slots))
				{
					try
					{
						slot.Listener.Invoke(item1, item2, item3, item4);
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
	}
}