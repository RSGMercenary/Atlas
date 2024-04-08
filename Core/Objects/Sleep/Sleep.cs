using Atlas.Core.Messages;

namespace Atlas.Core.Objects.Sleep;

internal class Sleep<T> : ISleep
	where T : ISleep, IMessenger<T>
{
	private readonly T Instance;
	private int sleeping = 0;

	public Sleep(T instance)
	{
		Instance = instance;
	}

	public int Sleeping
	{
		get => sleeping;
		set
		{
			if(sleeping == value)
				return;
			int previous = sleeping;
			sleeping = value;
			Instance.Message<ISleepMessage<T>>(new SleepMessage<T>(value, previous));
		}
	}

	public bool IsSleeping
	{
		get => sleeping > 0;
		set
		{
			if(value)
				++Sleeping;
			else
				--Sleeping;
		}
	}
}