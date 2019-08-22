using System;
using System.Diagnostics;

namespace Atlas.Core.Objects
{
	/// <summary>
	/// Updater is a class used to test the update loop of the IEngine in environments where you don't have
	/// a framework or other engine to provide an update loop for you. For example, in MonoGame the best practice
	/// is to put your AtlasEngine.Update() call directly into Game.Update() so MonoGame can provide the time.
	/// </summary>
	public class Updater
	{
		private readonly Stopwatch timer = new Stopwatch();
		private readonly Action<double> update;
		private bool isRunning = false;

		public Updater(Action<double> update)
		{
			this.update = update ?? throw new NullReferenceException("No method to call for updates.");
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
				//If the Updater is turned off and on during an Update()
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