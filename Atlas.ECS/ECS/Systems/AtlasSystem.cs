using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Systems;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasSystem : ISystem
{
	#region Events
	public event Action<ISystem, IEngine, IEngine> EngineChanged
	{
		add => EngineManager.EngineChanged += value;
		remove => EngineManager.EngineChanged -= value;
	}

	public event Action<ISystem, bool> IsUpdatingChanged
	{
		add => Updater.IsUpdatingChanged += value;
		remove => Updater.IsUpdatingChanged -= value;
	}

	public event Action<ISystem, int, int> SleepingChanged
	{
		add => Sleep.SleepingChanged += value;
		remove => Sleep.SleepingChanged -= value;
	}

	public event Action<ISystem, TimeStep, TimeStep> TimeStepChanged
	{
		add => Updater.TimeStepChanged += value;
		remove => Updater.TimeStepChanged -= value;
	}

	public event Action<ISystem, int, int> PriorityChanged;

	public event Action<ISystem, float, float> IntervalChanged;

	#endregion

	#region Fields
	private int priority = 0;
	private float totalIntervalTime = 0;
	private float deltaIntervalTime = 0;
	private readonly EngineManager<ISystem> EngineManager;
	private readonly Updater<ISystem> Updater;
	private readonly Sleep<ISystem> Sleep;
	#endregion

	#region Construct / Dispose
	protected AtlasSystem()
	{
		EngineManager = new(this, EngineChanging);
		Updater = new(this);
		Sleep = new(this);
		//Default all systems to be Variable.
		TimeStep = TimeStep.Variable;
	}

	public virtual void Dispose()
	{
		//Can't dispose System mid-update.
		if(Engine != null || Updater.IsUpdating)
			return;
		Disposing();
	}

	protected virtual void Disposing()
	{
		EngineManager.Dispose();
		Updater.Dispose();
		Sleep.Dispose();

		PriorityChanged = null;
		Priority = 0;

		IntervalChanged = null;
		DeltaIntervalTime = 0;

		TotalIntervalTime = 0;

	}
	#endregion

	#region Engine
	public IEngine Engine
	{
		get => EngineManager.Engine;
		set => EngineManager.Engine = value;
	}

	private void EngineChanging(IEngine current, IEngine previous)
	{
		if(previous != null)
		{
			RemovingEngine(previous);
			TotalIntervalTime = 0;
			Dispose();
		}
		if(current != null)
		{
			AddingEngine(current);
			SyncTotalIntervalTime();
		}
	}

	protected virtual void AddingEngine(IEngine engine) { }

	protected virtual void RemovingEngine(IEngine engine) { }
	#endregion

	#region Updates
	public void Update(float deltaTime)
	{
		if(IsSleeping)
			return;
		if(Engine != null && Engine.Updates.UpdateSystem != this)
			return;

		deltaTime = GetDeltaTime(deltaTime);
		if(deltaTime <= 0)
			return;

		Updater.Assert();

		Updater.IsUpdating = true;
		SystemUpdate(deltaTime);
		Updater.IsUpdating = false;

		if(Engine == null)
			Dispose();
	}

	protected virtual void SystemUpdate(float deltaTime) { }

	private float GetDeltaTime(float deltaTime)
	{
		if(deltaIntervalTime > 0)
		{
			if(GetEngineTime() - totalIntervalTime < deltaIntervalTime)
				return 0;
			TotalIntervalTime += deltaIntervalTime;
			return deltaIntervalTime;
		}
		return deltaTime;
	}

	public bool IsUpdating
	{
		get => Updater.IsUpdating;
		internal set => Updater.IsUpdating = value;
	}

	public TimeStep TimeStep
	{
		get => Updater.TimeStep;
		protected set
		{
			Updater.TimeStep = value;
			if(Engine != null)
				SyncTotalIntervalTime();
		}
	}
	#endregion

	#region Sleeping
	public int Sleeping
	{
		get => Sleep.Sleeping;
		private set => Sleep.Sleeping = value;
	}

	public bool IsSleeping
	{
		get => Sleep.IsSleeping;
		set => Sleep.IsSleeping = value;
	}
	#endregion

	#region Interval Time
	public float DeltaIntervalTime
	{
		get => deltaIntervalTime;
		protected set
		{
			if(deltaIntervalTime == value)
				return;
			var previous = deltaIntervalTime;
			deltaIntervalTime = value;
			IntervalChanged?.Invoke(this, value, previous);
			if(Engine != null)
				SyncTotalIntervalTime();
		}
	}

	public float TotalIntervalTime
	{
		get => totalIntervalTime;
		private set
		{
			if(totalIntervalTime == value)
				return;
			totalIntervalTime = value;
		}
	}

	/// <summary>
	/// Syncs this System's interval time to match other Systems with the same interval time.
	/// </summary>
	private void SyncTotalIntervalTime()
	{
		if(deltaIntervalTime <= 0)
			return;
		float totalIntervalTime = 0;
		while(totalIntervalTime + deltaIntervalTime <= GetEngineTime())
			totalIntervalTime += deltaIntervalTime;
		TotalIntervalTime = totalIntervalTime;
	}

	private double? GetEngineTime()
	{
		return TimeStep == TimeStep.Variable ? Engine?.Updates.TotalVariableTime : Engine?.Updates.TotalFixedTime;
	}
	#endregion

	#region Priority
	public int Priority
	{
		get => priority;
		set
		{
			if(priority == value)
				return;
			int previous = priority;
			priority = value;
			PriorityChanged?.Invoke(this, value, previous);
		}
	}
	#endregion
}