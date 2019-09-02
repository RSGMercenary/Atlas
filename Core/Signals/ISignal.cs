using System;
using System.Collections.Generic;

namespace Atlas.Core.Signals
{
	public interface ISignalBase
	{
		int Dispatching { get; }
		bool IsDispatching { get; }

		IReadOnlyList<ISlotBase> Slots { get; }

		ISlotBase Get(Delegate listener);
		ISlotBase Get(int index);
		int GetIndex(Delegate listener);

		ISlotBase Add(Delegate listener);
		ISlotBase Add(Delegate listener, int priority);

		bool Remove(Delegate listener);

		bool Remove(int index);
		bool RemoveAll();
	}

	public interface ISignalBase<out TSlot, in TDelegate> : ISignalBase
		where TSlot : ISlotBase
		where TDelegate : Delegate
	{
		TSlot Get(TDelegate listener);
		new TSlot Get(int index);
		int GetIndex(TDelegate listener);

		TSlot Add(TDelegate listener);
		TSlot Add(TDelegate listener, int priority);

		bool Remove(TDelegate listener);
	}

	public interface ISignal : ISignalBase<ISlot, Action> { }

	public interface ISignal<T1> : ISignalBase<ISlot<T1>, Action<T1>> { }

	public interface ISignal<T1, T2> : ISignalBase<ISlot<T1, T2>, Action<T1, T2>> { }

	public interface ISignal<T1, T2, T3> : ISignalBase<ISlot<T1, T2, T3>, Action<T1, T2, T3>> { }

	public interface ISignal<T1, T2, T3, T4> : ISignalBase<ISlot<T1, T2, T3, T4>, Action<T1, T2, T3, T4>> { }
}