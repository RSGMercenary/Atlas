using System;
using System.Diagnostics;
using System.Numerics;

namespace Atlas.Core.Objects.Update;

/// <summary>
/// Updater is a class used to test the update loop of the IEngine in environments where you don't have
/// a framework or other engine to provide an update loop for you. For example, in MonoGame the best practice
/// is to put your AtlasEngine.Update() call directly into Game.Update() so MonoGame can provide the time.
/// </summary>
public class UpdateRunner<T> where T : INumber<T>
{
	private readonly Stopwatch timer = new();
	private readonly IUpdate<T> instance;
	private bool isRunning = false;

	public UpdateRunner(IUpdate<T> instance)
	{
		this.instance = instance ?? throw new NullReferenceException($"{nameof(IUpdate<T>)} instance is null.");
	}

	public bool IsRunning
	{
		get => isRunning;
		set
		{
			if(isRunning == value)
				return;
			isRunning = value;
			//Only run again when the last Update()/timer is done.
			//If the Updater is turned off and on during an Update()
			//loop, while(isRunning) will catch it.
			if(value && !timer.IsRunning)
			{
				timer.Restart();
				var previousTime = T.Zero;
				while(isRunning)
				{
					var currentTime = T.CreateChecked(timer.Elapsed.TotalSeconds);
					instance.Update(currentTime - previousTime);
					previousTime = currentTime;
				}
				timer.Stop();
			}
		}
	}
}