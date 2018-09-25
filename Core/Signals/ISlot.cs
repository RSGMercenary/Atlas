using System;

namespace Atlas.Core.Signals
{
	public interface ISlotBase : IDisposable
	{
		ISignalBase Signal { get; }
		Delegate Listener { get; }
		int Priority { get; set; }
		Delegate Validator { get; set; }
	}

	public interface ISlotBase<out TSignal, out TListener, TValidator> : ISlotBase
		where TSignal : ISignalBase
		where TListener : Delegate
		where TValidator : Delegate
	{
		new TSignal Signal { get; }
		new TListener Listener { get; }
		new TValidator Validator { get; set; }
	}

	public interface ISlot : ISlotBase<ISignal, Action, Func<bool>>
	{

	}

	public interface ISlot<T1> : ISlotBase<ISignal<T1>, Action<T1>, Func<T1, bool>>
	{

	}

	public interface ISlot<T1, T2> : ISlotBase<ISignal<T1, T2>, Action<T1, T2>, Func<T1, T2, bool>>
	{

	}

	public interface ISlot<T1, T2, T3> : ISlotBase<ISignal<T1, T2, T3>, Action<T1, T2, T3>, Func<T1, T2, T3, bool>>
	{

	}

	public interface ISlot<T1, T2, T3, T4> : ISlotBase<ISignal<T1, T2, T3, T4>, Action<T1, T2, T3, T4>, Func<T1, T2, T3, T4, bool>>
	{

	}
}
