using System;
using System.Diagnostics;

namespace Atlas.ECS.Components.Engine
{
	public class EngineUpdater
	{
		private readonly Stopwatch timer = new Stopwatch();
		private readonly Action<double> update;
		private bool isRunning = false;

		public EngineUpdater(Action<double> update)
		{
			this.update = update ?? throw new NullReferenceException();
		}

		public bool IsRunning
		{
			get { return isRunning; }
			set
			{
				if(isRunning == value)
					return;
				isRunning = value;
				//Only run again when the last Update()/timer is done.
				//If the Engine is turned off and on during an Update()
				//loop, while(isRunning) will catch it.
				if(value && !timer.IsRunning)
				{
					timer.Restart();
					var previousTime = 0d;
					while(isRunning)
					{
						var currentTime = timer.Elapsed.TotalSeconds;
						update(currentTime - previousTime);
						previousTime = currentTime;
					}
					timer.Stop();
				}
			}
		}
	}
}
