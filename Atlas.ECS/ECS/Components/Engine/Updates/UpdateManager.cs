using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Components.Engine.Updates;

internal class UpdateManager : IUpdateManager, IUpdate<float>
{
	public event Action<IUpdateManager, TimeStep, TimeStep> UpdateStateChanged;

	//Update State
	private readonly UpdateLock UpdateLock = new();
	private TimeStep updateState = TimeStep.None;
	private ISystem updateSystem;

	//Variable Time
	private float deltaVariableTime = 0;
	private float totalVariableTime = 0;
	private float maxVariableTime = 0.25f;
	private float variableInterpolation = 0;

	//Fixed Time
	private float deltaFixedTime = 1f / 60f;
	private float totalFixedTime = 0;
	private int fixedUpdates = 0;
	private int fixedLag = 0;

	public IEngine Engine { get; }

	internal UpdateManager(IEngine engine)
	{
		Engine = engine;
	}

	#region Updates
	#region Delta / Total Times
	public float MaxVariableTime
	{
		get => maxVariableTime;
		set
		{
			if(maxVariableTime == value)
				return;
			maxVariableTime = value;
		}
	}

	[JsonIgnore]
	public float DeltaVariableTime
	{
		get => deltaVariableTime;
		private set
		{
			//Cap delta time to avoid the "spiral of death".
			value = float.Min(value, maxVariableTime);
			if(deltaVariableTime == value)
				return;
			deltaVariableTime = value;
		}
	}

	[JsonIgnore]
	public float TotalVariableTime
	{
		get => totalVariableTime;
		private set
		{
			if(totalVariableTime == value)
				return;
			totalVariableTime = value;
		}
	}

	public float DeltaFixedTime
	{
		get => deltaFixedTime;
		set
		{
			if(deltaFixedTime == value)
				return;
			deltaFixedTime = value;
		}
	}

	[JsonIgnore]
	public float TotalFixedTime
	{
		get => totalFixedTime;
		private set
		{
			if(totalFixedTime == value)
				return;
			totalFixedTime = value;
		}
	}
	#endregion

	#region State
	[JsonIgnore]
	public int FixedLag
	{
		get => fixedLag;
		private set => fixedLag = value;
	}

	[JsonIgnore]
	public int FixedUpdates
	{
		get => fixedUpdates;
		private set => fixedUpdates = value;
	}

	[JsonIgnore]
	public float VariableInterpolation
	{
		get => variableInterpolation;
		private set => variableInterpolation = value;
	}

	[JsonIgnore]
	public TimeStep UpdateState
	{
		get => updateState;
		private set
		{
			if(updateState == value)
				return;
			var previous = updateState;
			updateState = value;
			UpdateStateChanged?.Invoke(this, value, previous);
		}
	}

	[JsonIgnore]
	public ISystem UpdateSystem
	{
		get => updateSystem;
		private set
		{
			if(updateSystem == value)
				return;
			//If a Signal/Message were to ever be put here, do it before the set.
			//Prevents System.Update() from being mis-called.
			updateSystem = value;
		}
	}
	#endregion

	#region Fixed/Variable Update Loop
	public void Update(float deltaTime)
	{
		UpdateLock.Lock();

		//Variable-time cap and set.
		DeltaVariableTime = deltaTime;
		//Fixed-time cache to avoid modification during update.
		var deltaFixedTime = DeltaFixedTime;

		//Fixed-time updates
		CalculateFixedUpdates(deltaFixedTime);
		CalculateFixedLag();
		while(FixedUpdates > 0)
		{
			FixedUpdates--;
			TotalFixedTime += deltaFixedTime;
			UpdateSystems(TimeStep.Fixed, deltaFixedTime);
		}

		//Variable-time updates
		TotalVariableTime += deltaVariableTime;
		CalculateVariableInterpolation(deltaFixedTime);
		UpdateSystems(TimeStep.Variable, deltaVariableTime);

		UpdateLock.Unlock();
	}

	private void CalculateFixedUpdates(float deltaFixedTime)
	{
		if(deltaVariableTime <= 0)
			return;
		var fixedUpdates = 0;
		var totalFixedTime = TotalFixedTime;
		while(totalFixedTime + deltaFixedTime <= totalVariableTime + deltaVariableTime)
		{
			totalFixedTime += deltaFixedTime;
			++fixedUpdates;
		}
		FixedUpdates = fixedUpdates;
	}

	private void CalculateFixedLag()
	{
		//Calculate when fixed-time and variable-time updates weren't 1:1.
		var fixedLag = FixedLag + int.Max(0, fixedUpdates - 1);
		if(fixedUpdates == 1 && fixedLag > 0)
			--fixedLag;
		FixedLag = fixedLag;
	}

	private void CalculateVariableInterpolation(float deltaFixedTime)
	{
		VariableInterpolation = deltaFixedTime > 0 ? (TotalVariableTime - TotalFixedTime) / deltaFixedTime : 0;
	}

	private void UpdateSystems(TimeStep timeStep, float deltaTime)
	{
		UpdateState = timeStep;
		foreach(var system in Engine.Systems.Systems)
		{
			if(system.UpdateStep != timeStep)
				continue;
			UpdateSystem = system;
			system.Update(deltaTime);
			UpdateSystem = null;
		}
		UpdateState = TimeStep.None;
	}
	#endregion
	#endregion
}