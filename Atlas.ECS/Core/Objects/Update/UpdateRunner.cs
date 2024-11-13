using System;
using System.Diagnostics;
using System.Numerics;

namespace Atlas.Core.Objects.Update;

/// <summary>
/// <see cref="UpdateRunner{T}"/> provides an update loop to an <see cref="IUpdate{T}"/> instance.
/// </summary>
public sealed class UpdateRunner<T> : IUpdateRunner where T : INumber<T>
{
	public event Action<IUpdateRunner, bool> IsRunningChanged;

	private readonly Stopwatch timer = new();
	private readonly IUpdate<T> Instance;

	public UpdateRunner(IUpdate<T> instance)
	{
		Instance = instance ?? throw new NullReferenceException($"{nameof(IUpdate<T>)} instance is null.");
	}

	public bool IsRunning
	{
		get => field;
		set
		{
			if(field == value)
				return;
			field = value;
			IsRunningChanged?.Invoke(this, value);
			//Only run again when the last Update()/timer is done.
			//If the UpdateRunner is turned off and on during an Update()
			//loop, while(isRunning) will catch it.
			if(value && !timer.IsRunning)
			{
				timer.Restart();
				var previousTime = T.Zero;
				while(field)
				{
					var currentTime = T.CreateChecked(timer.Elapsed.TotalSeconds);
					Instance.Update(currentTime - previousTime);
					previousTime = currentTime;
				}
				timer.Stop();
			}
		}
	}
}