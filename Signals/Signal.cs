using System;
using System.Collections.Generic;

namespace Atlas.Signals
{
	class Signal
	{
		private List<Slot> slots = new List<Slot>();
		private List<Slot> slotsPooled = new List<Slot>();
		private List<Slot> slotsRemoved = new List<Slot>();

		private List<object[]> dispatchesPooled = new List<object[]>();
		private bool hasConcurrentDispatches = true;
		private int numDispatches = 0;

		public Signal(bool hasConcurrentDispatches = true)
		{
			HasConcurrentDispatches = hasConcurrentDispatches;
		}

		public bool HasConcurrentDispatches
		{
			get
			{
				return hasConcurrentDispatches;
			}
			private set
			{
				hasConcurrentDispatches = value;
			}
		}

		/// <summary>
		/// Cleans up the Signal by removing and disposing all listeners,
		/// and unpooling allocated Slots.
		/// </summary>
		public void Dispose()
		{
			RemoveAll();
			while(slotsPooled.Count > 0)
			{
				Slot slot = slotsPooled[slotsPooled.Count - 1];
				slotsPooled.RemoveAt(slotsPooled.Count - 1);
				slot.Dispose();
			}
		}

		/// <summary>
		/// Calls Dispose only if there are no listeners.
		/// </summary>
		public bool DisposeIfEmpty()
		{
			if(slots.Count <= 0)
			{
				Dispose();
				return true;
			}
			return false;
		}

		public void Dispatch(params object[] values)
		{
			if(slots.Count > 0)
			{
				if(!hasConcurrentDispatches && numDispatches > 0)
				{
					dispatchesPooled.Add(values);
				}
				else
				{
					++numDispatches;

					foreach(Slot slot in Slots)
					{
						slot.Listener.DynamicInvoke(values);
					}

					--numDispatches;

					if(numDispatches == 0)
					{
						while(slotsRemoved.Count > 0)
						{
							Slot slot = slotsRemoved[slotsRemoved.Count - 1];
							slotsRemoved.RemoveAt(slotsRemoved.Count - 1);
							DisposeSlot(slot);
						}

						if(dispatchesPooled.Count > 0)
						{
							values = dispatchesPooled[0];
							dispatchesPooled.RemoveAt(0);
							Dispatch(values);
						}
					}
				}
			}
		}

		protected bool DispatchStart()
		{
			if(slots.Count > 0)
			{
				++numDispatches;
				return true;
			}
			return false;
		}

		protected bool DispatchStop()
		{
			--numDispatches;
			if(numDispatches == 0)
			{
				while(slotsRemoved.Count > 0)
				{
					Slot slot = slotsRemoved[slotsRemoved.Count - 1];
					slotsRemoved.RemoveAt(slotsRemoved.Count - 1);
					DisposeSlot(slot);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// The number of concurrent dispatches. During a dispatch, it's possible that external
		/// code could require another dispatch on the same Signal.
		/// </summary>
		public int NumDispatches
		{
			get
			{
				return numDispatches;
			}
		}

		/// <summary>
		/// The number of Slots/listeners attached to this Signal.
		/// </summary>
		public int NumSlots
		{
			get
			{
				return slots.Count;
			}
		}

		/// <summary>
		/// Returns a copy of the Slots being processed by this Signal in order of
		/// how they're prioritized.
		/// </summary>
		public List<Slot> Slots
		{
			get
			{
				return new List<Slot>(slots);
			}
		}

		private void DisposeSlot(Slot slot)
		{
			slot.Signal = null;
			slot.Dispose();
			slotsPooled.Add(slot);
		}

		public Slot Get(Delegate listener)
		{
			if(listener != null)
			{
				foreach(Slot slot in slots)
				{
					if(slot.Listener == listener)
					{
						return slot;
					}
				}
			}
			return null;
		}

		public Slot GetAt(int index)
		{
			if(index < 0)
				return null;
			if(index > slots.Count - 1)
				return null;
			return slots[index];
		}

		public int GetIndex(Delegate listener)
		{
			if(listener != null)
			{
				for(int index = slots.Count - 1; index > -1; --index)
				{
					if(slots[index].Listener == listener)
					{
						return index;
					}
				}
			}
			return -1;
		}

		public Slot Add(Delegate listener, int priority = 0, bool isOnce = false)
		{
			if(listener != null)
			{
				Slot slot = Get(listener);
				if(slot == null)
				{
					if(slotsPooled.Count > 0)
					{
						slot = slots[slotsPooled.Count - 1];
						slotsPooled.RemoveAt(slotsPooled.Count - 1);
					}
					else
					{
						slot = CreateSlot();
					}

					slot.Signal = this;
					slot.Listener = listener;
					slot.Priority = priority;
					slot.IsOnce = isOnce;

					PriorityChanged(slot, 0, 0);

					return slot;
				}
			}
			return null;
		}

		virtual protected Slot CreateSlot()
		{
			return new Slot();
		}

		internal void PriorityChanged(Slot slot, int current, int previous)
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
						return RemoveAt(index);
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
		public bool RemoveAt(int index)
		{
			if(index > 0 && index < slots.Count)
			{
				Slot slot = slots[index];
				slots.RemoveAt(index);

				if(numDispatches > 0)
				{
					slotsRemoved.Add(slot);
				}
				else
				{
					DisposeSlot(slot);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Removes all Slots/listeners.
		/// </summary>
		public void RemoveAll()
		{
			while(slots.Count > 0)
			{
				RemoveAt(slots.Count - 1);
			}
		}
	}
}