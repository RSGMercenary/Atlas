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

	public event Action<ISystem, int, int> SleepingChanged
	{
		add => Sleep.SleepingChanged += value;
		remove => Sleep.SleepingChanged -= value;
	}

	public event Action<ISystem, UpdatePhase, UpdatePhase> UpdatePhaseChanged;

	public event Action<ISystem, TimeStep, TimeStep> TimeStepChanged;

	public event Action<ISystem, float, float> IntervalChanged;

	public event Action<ISystem, int, int> PriorityChanged;
	#endregion

	#region Fields
	private int priority = 0;
	private float totalIntervalTime = 0;
	private float deltaIntervalTime = 0;
	private TimeStep timeStep = TimeStep.Variable;
	private UpdatePhase updatePhase = UpdatePhase.UpdateEnd;
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
		if(Engine != null || updatePhase != UpdatePhase.UpdateEnd)
			return;
		Disposing();
	}

	protected virtual void Disposing()
	{
		EngineManager.Dispose();
		Sleep.Dispose();

		PriorityChanged = null;
		Priority = 0;

		UpdatePhaseChanged = null;
		UpdatePhase = UpdatePhase.UpdateEnd;

		TimeStepChanged = null;
		TimeStep = TimeStep.Variable;

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

		UpdateLock.Lock();

		UpdatePhase = UpdatePhase.UpdateStart;
		SystemUpdate(deltaTime);
		UpdatePhase = UpdatePhase.UpdateEnd;

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

	public UpdatePhase UpdatePhase
	{
		get => updatePhase;
		internal set
		{
			if(updatePhase == value)
				return;
			var previous = updatePhase;
			updatePhase = value;
			UpdatePhaseChanged?.Invoke(this, value, previous);
		}
	}

	public TimeStep TimeStep
	{
		get => timeStep;
		protected set
		{
			if(timeStep == value)
				return;
			var previous = timeStep;
			timeStep = value;
			TimeStepChanged?.Invoke(this, value, previous);
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
		return timeStep == TimeStep.Variable ? Engine?.Updates.TotalVariableTime : Engine?.Updates.TotalFixedTime;
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