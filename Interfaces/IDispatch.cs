namespace Atlas.Interfaces
{
	interface IDispatchDynamic
	{
		void Dispatch(params object[] items);
	}

	interface IDispatch
	{
		void Dispatch();
	}

	interface IDispatch<T1>
	{
		void Dispatch(T1 item1);
	}

	interface IDispatch<T1, T2>
	{
		void Dispatch(T1 item1, T2 item2);
	}

	interface IDispatch<T1, T2, T3>
	{
		void Dispatch(T1 item1, T2 item2, T3 item3);
	}

	interface IDispatch<T1, T2, T3, T4>
	{
		void Dispatch(T1 item1, T2 item2, T3 item3, T4 item4);
	}

	interface IDispatch<T1, T2, T3, T4, T5>
	{
		void Dispatch(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5);
	}
}
