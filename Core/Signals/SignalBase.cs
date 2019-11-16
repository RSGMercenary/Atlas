using Atlas.Core.Collections.Group;
using System;
using System.Collections.Generic;

namespace Atlas.Core.Signals
{
	public abstract class SignalBase : ISignalBase, IDisposable
	{
		private readonly Group<SlotBase> slots = new Group<SlotBase>();
		private readonly Stack<SlotBase> slotsPooled = new Stack<SlotBase>();
		private readonly Stack<SlotBase> slotsRemoved = new Stack<SlotBase>();

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

		public bool IsDispatching => Dispatching > 0;

		public IReadOnlyList<ISlotBase> Slots => slots;

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

		public ISlotBase Add(Delegate listener) => Add(listener, 0);

		public ISlotBase Add(Delegate listener, int priority)
		{
			if(listener == null)
				return null;
			var slot = Get(listener) as SlotBase;
			if(slot == null)
			{
				if(slotsPooled.Count > 0)
					slot = slotsPooled.Pop();
				else
					slot = CreateSlot();
			}

			slot.Signal = this;
			slot.Listener = listener;
			slot.Priority = priority;

			Prioritize(slot);

			IsDisposed = false;

			return slot;
		}

		protected abstract SlotBase CreateSlot();

		internal void Prioritize(SlotBase slot)
		{
			slots.Remove(slot);
			for(var index = slots.Count; index > 0; --index)
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
				Remove(slots.Count - 1);
			return true;
		}

		protected bool Dispatch<TSlot>(Action<TSlot> dispatcher)
			where TSlot : ISlotBase
		{
			if(slots.Count <= 0)
				return false;
			++Dispatching;
			foreach(TSlot slot in Slots)
				dispatcher.Invoke(slot);
			if(--Dispatching == 0)
			{
				while(slotsRemoved.Count > 0)
					DisposeSlot(slotsRemoved.Pop());
			}
			return true;
		}
	}

	public abstract class SignalBase<TSlot, TISlot, TDelegate> : SignalBase, ISignalBase<TISlot, TDelegate>
		where TSlot : SlotBase, TISlot, new()
		where TISlot : ISlotBase
		where TDelegate : Delegate
	{
		public TISlot Add(TDelegate listener) => (TISlot)base.Add(listener, 0);

		public TISlot Add(TDelegate listener, int priority) => (TISlot)base.Add(listener, priority);

		public TISlot Get(TDelegate listener) => (TISlot)base.Get(listener);

		public int GetIndex(TDelegate listener) => base.GetIndex(listener);

		public bool Remove(TDelegate listener) => base.Remove(listener);

		public new TISlot Get(int index) => (TISlot)base.Get(index);

		protected override SlotBase CreateSlot() => CreateGenericSlot();

		protected virtual TSlot CreateGenericSlot() => new TSlot();

		protected bool Dispatch(Action<TSlot> dispatcher) => Dispatch<TSlot>(dispatcher);
	}
}