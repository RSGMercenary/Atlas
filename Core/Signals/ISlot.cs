using System;

namespace Atlas.Core.Signals
{
	public interface ISlotBase : IDisposable
	{
		ISignalBase Signal { get; }
		Delegate Listener { get; }
		int Priority { get; set; }
	}

	public interface ISlotBase<out TSignal, out TListener> : ISlotBase
		where TSignal : ISignalBase
		where TListener : Delegate
	{
		new TSignal Signal { get; }
		new TListener Listener { get; }
	}

	public interface ISlot : ISlotBase<ISignal, Action>
	{

	}

	public interface ISlot<T1> : ISlotBase<ISignal<T1>, Action<T1>>
	{

	}

	public interface ISlot<T1, T2> : ISlotBase<ISignal<T1, T2>, Action<T1, T2>>
	{

	}

	public interface ISlot<T1, T2, T3> : ISlotBase<ISignal<T1, T2, T3>, Action<T1, T2, T3>>
	{

	}

	public interface ISlot<T1, T2, T3, T4> : ISlotBase<ISignal<T1, T2, T3, T4>, Action<T1, T2, T3, T4>>
	{

	}
}