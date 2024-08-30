using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Components.Engine.Updates;

internal class UpdateManager : IUpdateManager, IUpdate<float>
{
	#region Events
	public event Action<IUpdateManager, bool> IsUpdatingChanged
	{
		add => Updater.IsUpdatingChanged += value;
		remove => Updater.IsUpdatingChanged -= value;
	}

	public event Action<IUpdateManager, TimeStep, TimeStep> TimeStepChanged
	{
		add => Updater.TimeStepChanged += value;
		remove => Updater.TimeStepChanged -= value;
	}

	public event Action<IUpdateManager, float, float> MaxVariableTimeChanged;
	public event Action<IUpdateManager, float, float> DeltaFixedTimeChanged;
	#endregion

	#region Fields
	//Updates
	private readonly Updater<IUpdateManager> Updater;
	private ISystem updateSystem;

	//Fixed Time
	private float deltaFixedTime = 1f / 60f;
	private float totalFixedTime = 0;
	private int fixedUpdates = 0;
	private int fixedLag = 0;

	//Variable Time
	private float deltaVariableTime = 0;
	private float totalVariableTime = 0;
	private float maxVariableTime = 0.25f;
	private float variableInterpolation = 0;
	#endregion

	internal UpdateManager(IEngine engine)
	{
		Engine = engine;
		Updater = new(this);
	}

	public IEngine Engine { get; }

	#region Updates
	[JsonIgnore]
	public bool IsUpdating
	{
		get => Updater.IsUpdating;
		private set => Updater.IsUpdating = value;
	}

	[JsonIgnore]
	public TimeStep TimeStep
	{
		get => Updater.TimeStep;
		private set => Updater.TimeStep = value;
	}

	#region Delta / Total Times
	public float MaxVariableTime
	{
		get => maxVariableTime;
		set
		{
			if(maxVariableTime == value)
				return;
			var previous = maxVariableTime;
			maxVariableTime = value;
			MaxVariableTimeChanged?.Invoke(this, value, previous);
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
			var previous = deltaFixedTime;
			deltaFixedTime = value;
			DeltaFixedTimeChanged?.Invoke(this, value, previous);
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
	public ISystem UpdateSystem
	{
		get => updateSystem;
		private set
		{
			if(updateSystem == value)
				return;
			//If an event were to ever be put here, do it before the set.
			//Prevents System.Update() from being mis-called.
			updateSystem = value;
		}
	}
	#endregion

	#region Update
	public void Update(float deltaTime)
	{
		Updater.Assert();
		IsUpdating = true;

		//Variable-time cap and set.
		DeltaVariableTime = deltaTime;
		//Fixed-time cache to avoid modification during update.
		var deltaFixedTime = DeltaFixedTime;

		//Fixed-time updates
		CalculateFixedUpdates(deltaFixedTime);
		CalculateFixedLag();
		TimeStep = TimeStep.Fixed;

		while(FixedUpdates > 0)
		{
			FixedUpdates--;
			TotalFixedTime += deltaFixedTime;
			UpdateSystems(TimeStep.Fixed, deltaFixedTime);
		}

		//Variable-time updates
		TotalVariableTime += deltaVariableTime;
		CalculateVariableInterpolation(deltaFixedTime);
		TimeStep = TimeStep.Variable;

		UpdateSystems(TimeStep.Variable, deltaVariableTime);

		TimeStep = TimeStep.None;
		IsUpdating = false;
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
		for(var current = Engine.Systems.Systems.First; current != null; current = current.Next)
		{
			var system = current.Value;

			if(system.TimeStep != timeStep)
				continue;
			UpdateSystem = system;
			system.Update(deltaTime);
			UpdateSystem = null;
		}
	}
	#endregion
	#endregion
}