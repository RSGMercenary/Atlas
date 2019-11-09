namespace Atlas.Core.Signals
{
	public interface IDispatch
	{
		bool Dispatch();
	}

	public interface IDispatch<in T1>
	{
		bool Dispatch(T1 item1);
	}

	public interface IDispatch<in T1, in T2>
	{
		bool Dispatch(T1 item1, T2 item2);
	}

	public interface IDispatch<in T1, in T2, in T3>
	{
		bool Dispatch(T1 item1, T2 item2, T3 item3);
	}

	public interface IDispatch<in T1, in T2, in T3, in T4>
	{
		bool Dispatch(T1 item1, T2 item2, T3 item3, T4 item4);
	}
}