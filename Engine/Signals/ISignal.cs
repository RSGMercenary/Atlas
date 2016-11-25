using Atlas.Engine.Interfaces;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Signals
{
	interface ISignalBase
	{
		int Dispatching { get; }
		bool IsDispatching { get; }

		int NumSlots { get; }
		bool IsEmpty { get; }

		IReadOnlyList<ISlotBase> Slots { get; }
		List<ISlotBase> SlotsCopy { get; }

		ISlotBase Get(Delegate listener);
		ISlotBase Get(int index);
		int GetIndex(Delegate listener);

		ISlotBase Add(Delegate listener);
		ISlotBase Add(Delegate listener, int priority = 0);

		bool Remove(Delegate listener);
		bool Remove(int index);
		bool RemoveAll();
	}

	interface ISignalDynamic:ISignalBase, IDispatchDynamic
	{

	}

	interface ISignal:ISignalBase
	{
		//new List<ISlot> SlotsCopy { get; }

		ISlot Get(Action listener);
		new ISlot Get(int index);
		int GetIndex(Action listener);

		ISlot Add(Action listener);
		ISlot Add(Action listener, int priority = 0);

		bool Remove(Action listener);
	}

	interface ISignal<T1>:ISignalBase
	{
		//new List<ISlot<T1>> SlotsCopy { get; }

		ISlot<T1> Get(Action<T1> listener);
		new ISlot<T1> Get(int index);
		int GetIndex(Action<T1> listener);

		ISlot<T1> Add(Action<T1> listener);
		ISlot<T1> Add(Action<T1> listener, int priority = 0);

		bool Remove(Action<T1> listener);
	}

	interface ISignal<T1, T2>:ISignalBase
	{
		//new List<ISlot<T1, T2>> SlotsCopy { get; }

		ISlot<T1, T2> Get(Action<T1, T2> listener);
		new ISlot<T1, T2> Get(int index);
		int GetIndex(Action<T1, T2> listener);

		ISlot<T1, T2> Add(Action<T1, T2> listener);
		ISlot<T1, T2> Add(Action<T1, T2> listener, int priority = 0);

		bool Remove(Action<T1, T2> listener);
	}

	interface ISignal<T1, T2, T3>:ISignalBase
	{
		//new List<ISlot<T1, T2, T3>> SlotsCopy { get; }

		ISlot<T1, T2, T3> Get(Action<T1, T2, T3> listener);
		new ISlot<T1, T2, T3> Get(int index);
		int GetIndex(Action<T1, T2, T3> listener);

		ISlot<T1, T2, T3> Add(Action<T1, T2, T3> listener);
		ISlot<T1, T2, T3> Add(Action<T1, T2, T3> listener, int priority = 0);

		bool Remove(Action<T1, T2, T3> listener);
	}

	interface ISignal<T1, T2, T3, T4>:ISignalBase
	{
		//new List<ISlot<T1, T2, T3, T4>> SlotsCopy { get; }

		ISlot<T1, T2, T3, T4> Get(Action<T1, T2, T3, T4> listener);
		new ISlot<T1, T2, T3, T4> Get(int index);
		int GetIndex(Action<T1, T2, T3, T4> listener);

		ISlot<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> listener);
		ISlot<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> listener, int priority = 0);

		bool Remove(Action<T1, T2, T3, T4> listener);
	}
}
