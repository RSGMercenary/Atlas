using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Systems;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasSystem : ISystem
{
	#region Fields
	private int priority = 0;
	private float totalIntervalTime = 0;
	private float deltaIntervalTime = 0;
	private TimeStep timeStep = TimeStep.Variable;
	private TimeStep updateState = TimeStep.None;
	private readonly EngineManager<ISystem> EngineManager;
	private readonly UpdateLock UpdateLock;
	private readonly Sleep<ISystem> Sleep;
	#endregion

	#region Construct / Dispose
	protected AtlasSystem()
	{
		EngineManager = new(this, EngineChanging);
		UpdateLock = new();
		Sleep = new(this);
	}

	public virtual void Dispose()
	{
		//Can't dispose System mid-update.
		if(Engine != null || updateState != TimeStep.None)
			return;
		Disposing();
	}

	protected virtual void Disposing()
	{
		EngineManager.Dispose();
		Sleep.Dispose();

		PriorityChanged = null;
		Priority = 0;

		UpdateStateChanged = null;
		UpdateState = TimeStep.None;

		UpdateStepChanged = null;
		UpdateStep = TimeStep.Variable;

		IntervalChanged = null;
		DeltaIntervalTime = 0;

		TotalIntervalTime = 0;

	}
	#endregion

	#region Engine
	public event Action<ISystem, IEngine, IEngine> EngineChanged
	{
		add => EngineManager.EngineChanged += value;
		remove => EngineManager.EngineChanged -= value;
	}

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

		UpdateLock.Lock();

		UpdateState = UpdateStep;
		SystemUpdate(deltaTime);
		UpdateState = TimeStep.None;

		UpdateLock.Unlock();

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

	public event Action<ISystem, TimeStep, TimeStep> UpdateStateChanged;

	public TimeStep UpdateState
	{
		get => updateState;
		internal set
		{
			if(updateState == value)
				return;
			var previous = updateState;
			updateState = value;
			UpdateStateChanged?.Invoke(this, value, previous);
		}
	}

	public event Action<ISystem, TimeStep, TimeStep> UpdateStepChanged;

	public TimeStep UpdateStep
	{
		get => timeStep;
		protected set
		{
			if(timeStep == value)
				return;
			var previous = timeStep;
			timeStep = value;
			UpdateStepChanged?.Invoke(this, value, previous);
			if(Engine != null)
				SyncTotalIntervalTime();
		}
	}
	#endregion

	#region Sleeping
	public event Action<ISystem, int, int> SleepingChanged
	{
		add => Sleep.SleepingChanged += value;
		remove => Sleep.SleepingChanged -= value;
	}

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
	public event Action<ISystem, float, float> IntervalChanged;

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
		return timeStep == TimeStep.Variable ? Engine?.Updates.TotalVariableTime : Engine?.Updates.TotalFixedTime;
	}
	#endregion

	#region Priority
	public event Action<ISystem, int, int> PriorityChanged;

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