namespace Atlas.Core.Signals
{
	public interface IDispatch
	{
		bool Dispatch();
	}

	public interface IDispatch<T1>
	{
		bool Dispatch(T1 item1);
	}

	public interface IDispatch<T1, T2>
	{
		bool Dispatch(T1 item1, T2 item2);
	}

	public interface IDispatch<T1, T2, T3>
	{
		bool Dispatch(T1 item1, T2 item2, T3 item3);
	}

	public interface IDispatch<T1, T2, T3, T4>
	{
		bool Dispatch(T1 item1, T2 item2, T3 item3, T4 item4);
	}
}