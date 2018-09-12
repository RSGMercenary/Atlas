using Atlas.Framework.Collections.EngineList;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atlas.Framework.Signals
{
	public class SignalBase : ISignalBase, IDisposable
	{
		public static implicit operator bool(SignalBase signal)
		{
			return signal != null;
		}

		private EngineList<SlotBase> slots = new EngineList<SlotBase>();
		private Stack<SlotBase> slotsPooled = new Stack<SlotBase>();
		private Stack<SlotBase> slotsRemoved = new Stack<SlotBase>();

		public int Dispatching { get; private set; } = 0;
		public bool IsDisposed { get; private set; } = false;

		/// <summary>
		/// Cleans up the Signal by removing and disposing all listeners,
		/// and unpooling allocated Slots.
		/// </summary>
		public void Dispose()
		{
			if(IsDisposed)
				return;
			IsDisposed = true;
			slotsPooled.Clear();
			RemoveAll();
		}

		public bool IsDispatching
		{
			get { return Dispatching > 0; }
		}

		public IReadOnlyList<ISlotBase> Slots
		{
			get { return slots; }
		}

		private void DisposeSlot(SlotBase slot)
		{
			slot.Signal = null;
			slot.Dispose();
			if(!IsDisposed)
				slotsPooled.Push(slot);
		}

		public ISlotBase Get(Delegate listener)
		{
			if(listener == null)
				return null;
			foreach(var slot in slots)
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
			slot.Listener = listener;
			slot.Priority = priority;

			PriorityChanged(slot, 0, 0);

			IsDisposed = false;

			return slot;
		}

		protected virtual SlotBase CreateSlot()
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
			var slot = slots[index];
			slots.RemoveAt(index);
			if(Dispatching > 0)
			{
				slotsRemoved.Push(slot);
			}
			else
			{
				DisposeSlot(slot);
			}
			return true;
		}

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

		protected bool Dispatch<TSlot>(Action<TSlot> dispatcher)
			where TSlot : ISlotBase
		{
			if(slots.Count > 0)
			{
				++Dispatching;
				foreach(TSlot slot in Slots)
				{
					try
					{
						dispatcher.Invoke(slot);
					}
					catch(Exception e)
					{
						Debug.WriteLine(e);
						//We remove the Slot so the Error doesn't inevitably happen again.
						//TO-DO Might not wanna do this.
						Remove(slot.Listener);
					}
				}
				if(--Dispatching == 0)
				{
					while(slotsRemoved.Count > 0)
						DisposeSlot(slotsRemoved.Pop());
				}
				return true;
			}
			return false;
		}
	}

	public class SignalBase<TSlot, TISlot, TDelegate> : SignalBase, ISignalBase<TISlot, TDelegate>
		where TSlot : SlotBase, TISlot, new()
		where TISlot : class, ISlotBase
		where TDelegate : Delegate
	{
		public SignalBase()
		{

		}

		public TISlot Add(TDelegate listener)
		{
			return base.Add(listener) as TISlot;
		}

		public TISlot Add(TDelegate listener, int priority)
		{
			return base.Add(listener, priority) as TISlot;
		}

		public TISlot Get(TDelegate listener)
		{
			return base.Get(listener) as TISlot;
		}

		public int GetIndex(TDelegate listener)
		{
			return base.GetIndex(listener);
		}

		public bool Remove(TDelegate listener)
		{
			return base.Remove(listener);
		}

		public new TISlot Get(int index)
		{
			return base.Get(index) as TISlot;
		}

		protected override SlotBase CreateSlot()
		{
			return new TSlot();
		}

		protected bool Dispatch(Action<TSlot> dispatcher)
		{
			return Dispatch<TSlot>(slot => dispatcher.Invoke(slot));
		}
	}
}