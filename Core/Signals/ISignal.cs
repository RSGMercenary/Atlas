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
		ISlotBase Add(Delegate listener, Delegate validator);
		ISlotBase Add(Delegate listener, int priority, Delegate validator);

		bool Remove(Delegate listener);

		bool Remove(int index);
		bool RemoveAll();
	}

	public interface ISignalBase<TSlot, TDelegate, TValidator> : ISignalBase
		where TSlot : class, ISlotBase
		where TDelegate : Delegate
		where TValidator : Delegate
	{
		TSlot Get(TDelegate listener);
		new TSlot Get(int index);
		int GetIndex(TDelegate listener);

		TSlot Add(TDelegate listener);
		TSlot Add(TDelegate listener, int priority);
		TSlot Add(TDelegate listener, TValidator validator);
		TSlot Add(TDelegate listener, int priority, TValidator validator);

		bool Remove(TDelegate listener);
	}

	public interface ISignal : ISignalBase<ISlot, Action, Func<bool>>
	{

	}

	public interface ISignal<T1> : ISignalBase<ISlot<T1>, Action<T1>, Func<T1, bool>>
	{

	}

	public interface ISignal<T1, T2> : ISignalBase<ISlot<T1, T2>, Action<T1, T2>, Func<T1, T2, bool>>
	{

	}

	public interface ISignal<T1, T2, T3> : ISignalBase<ISlot<T1, T2, T3>, Action<T1, T2, T3>, Func<T1, T2, T3, bool>>
	{

	}

	public interface ISignal<T1, T2, T3, T4> : ISignalBase<ISlot<T1, T2, T3, T4>, Action<T1, T2, T3, T4>, Func<T1, T2, T3, T4, bool>>
	{

	}
}
