namespace Atlas.Core.Signals
{
	interface IDispatch
	{
		bool Dispatch();
	}

	interface IDispatch<T1>
	{
		bool Dispatch(T1 item1);
	}

	interface IDispatch<T1, T2>
	{
		bool Dispatch(T1 item1, T2 item2);
	}

	interface IDispatch<T1, T2, T3>
	{
		bool Dispatch(T1 item1, T2 item2, T3 item3);
	}

	interface IDispatch<T1, T2, T3, T4>
	{
		bool Dispatch(T1 item1, T2 item2, T3 item3, T4 item4);
	}
}