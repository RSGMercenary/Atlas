using System;
using System.Collections.Generic;

namespace Atlas.Engine.Signals
{
	public class SignalBase : ISignalBase, IDisposable
	{
		public static implicit operator bool(SignalBase signal)
		{
			return signal != null;
		}

		private List<SlotBase> slots = new List<SlotBase>();
		private Stack<SlotBase> slotsPooled = new Stack<SlotBase>();
		private Stack<SlotBase> slotsRemoved = new Stack<SlotBase>();
		private int dispatching = 0;
		private bool isDisposed = false;

		/// <summary>
		/// Cleans up the Signal by removing and disposing all listeners,
		/// and unpooling allocated Slots.
		/// </summary>
		public void Dispose()
		{
			if(isDisposed)
				return;
			isDisposed = true;
			slotsPooled.Clear();
			RemoveAll();
		}

		public bool IsDisposed
		{
			get { return isDisposed; }
		}

		public bool HasListeners
		{
			get { return slots.Count > 0; }
		}

		public bool IsDispatching
		{
			get { return dispatching > 0; }
		}

		protected bool DispatchStart()
		{
			if(slots.Count > 0)
			{
				++dispatching;
				return true;
			}
			return false;
		}

		protected bool DispatchStop()
		{
			if(dispatching <= 0)
				return false;
			if(--dispatching == 0)
			{
				while(slotsRemoved.Count > 0)
				{
					DisposeSlot(slotsRemoved.Pop());
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// The number of concurrent dispatches. During a dispatch, it's possible that external
		/// code could require another dispatch on the same Signal.
		/// </summary>
		public int Dispatching
		{
			get { return dispatching; }
		}

		public IReadOnlyList<ISlotBase> Slots
		{
			get { return slots; }
		}

		private void DisposeSlot(SlotBase slot)
		{
			slot.Signal = null;
			slot.Dispose();
			if(!isDisposed)
				slotsPooled.Push(slot);
		}

		public ISlotBase Get(Delegate listener)
		{
			if(listener == null)
				return null;
			foreach(SlotBase slot in slots)
			{
				if(slot.Listener == listener)
					return slot;
			}
			return null;
		}

		public ISlotBase Get(int index)
		{
			if(index < 0)
				return null;
			if(index > slots.Count - 1)
				return null;
			return slots[index];
		}

		public int GetIndex(Delegate listener)
		{
			if(listener == null)
				return -1;
			for(int index = slots.Count - 1; index > -1; --index)
			{
				if(slots[index].Listener == listener)
					return index;
			}
			return -1;
		}

		public ISlotBase Add(Delegate listener)
		{
			return Add(listener, 0);
		}

		public ISlotBase Add(Delegate listener, int priority = 0)
		{
			if(listener == null)
				return null;
			SlotBase slot = Get(listener) as SlotBase;
			if(!slot)
			{
				if(slotsPooled.Count > 0)
				{
					slot = slotsPooled.Pop();
				}
				else
				{
					slot = CreateSlot();
				}
			}

			slot.Signal = this;
			slot.Listener = listener as Delegate;
			slot.Priority = priority;

			PriorityChanged(slot, 0, 0);

			isDisposed = false;

			return slot;
		}

		virtual protected SlotBase CreateSlot()
		{
			return new SlotBase();
		}

		internal void PriorityChanged(SlotBase slot, int current, int previous)
		{
			slots.Remove(slot);

			for(int index = slots.Count; index > 0; --index)
			{
				if(slots[index - 1].Priority <= slot.Priority)
				{
					slots.Insert(index, slot);
					return;
				}
			}

			slots.Insert(0, slot);
		}

		public bool Remove(Delegate listener)
		{
			if(listener != null)
			{
				for(int index = slots.Count - 1; index > -1; --index)
				{
					if(slots[index].Listener == listener)
					{
						return Remove(index);
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Removes the Slot/listener at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool Remove(int index)
		{
			if(index < 0)
				return false;
			if(index >= slots.Count)
				return false;
			SlotBase slot = slots[index];
			slots.RemoveAt(index);
			if(dispatching > 0)
			{
				slotsRemoved.Push(slot);
			}
			else
			{
				DisposeSlot(slot);
			}
			return true;
		}

		/// <summary>
		/// Removes all SlotsCopy/listeners.
		/// </summary>
		public bool RemoveAll()
		{
			if(slots.Count <= 0)
				return false;
			while(slots.Count > 0)
			{
				Remove(slots.Count - 1);
			}
			return true;
		}
	}

	public class SignalBase<TSlot, TISlot, TDelegate> : SignalBase, ISignalBase<TISlot, TDelegate>
		where TSlot : SlotBase, TISlot, new()
		where TISlot : class, ISlotBase
		where TDelegate : class
	{
		public SignalBase()
		{

		}

		public TISlot Add(TDelegate listener)
		{
			return Add(listener as Delegate) as TISlot;
		}

		public TISlot Add(TDelegate listener, int priority = 0)
		{
			return Add(listener as Delegate) as TISlot;
		}

		public TISlot Get(TDelegate listener)
		{
			return Get(listener as Delegate) as TISlot;
		}

		public int GetIndex(TDelegate listener)
		{
			return GetIndex(listener as Delegate);
		}

		public bool Remove(TDelegate listener)
		{
			return Remove(listener as Delegate);
		}

		public new TISlot Get(int index)
		{
			return base.Get(index) as TISlot;
		}

		sealed protected override SlotBase CreateSlot()
		{
			return new TSlot();
		}
	}
}