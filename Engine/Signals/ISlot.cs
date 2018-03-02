using Atlas.Engine.Interfaces;
using System;

namespace Atlas.Engine.Signals
{
	public interface ISlotBase : IDisposable, IPriority
	{
		ISignalBase Signal { get; }
		Delegate Listener { get; }
	}

	public interface ISlotBase<TSignal, TDelegate> : ISlotBase
		where TSignal : class, ISignalBase
		where TDelegate : class
	{
		new TSignal Signal { get; }
		new TDelegate Listener { get; }
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
