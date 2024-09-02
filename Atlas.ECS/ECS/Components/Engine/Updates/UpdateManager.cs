﻿using Atlas.Core.Collections.LinkList;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using System;

namespace Atlas.ECS.Components.Engine.Updates;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
internal sealed class UpdateManager : IUpdateManager, IUpdate<float>
{
	#region Events
	public event Action<IUpdateManager, bool> IsUpdatingChanged
	{
		add => Updater.IsUpdatingChanged += value;
		remove => Updater.IsUpdatingChanged -= value;
	}

	event Action<IUpdater, bool> IUpdater.IsUpdatingChanged
	{
		add => Updater.IsUpdatingChanged += value;
		remove => Updater.IsUpdatingChanged -= value;
	}

	public event Action<IUpdateManager, TimeStep, TimeStep> TimeStepChanged
	{
		add => Updater.TimeStepChanged += value;
		remove => Updater.TimeStepChanged -= value;
	}

	event Action<IUpdater, TimeStep, TimeStep> IUpdater.TimeStepChanged
	{
		add => Updater.TimeStepChanged += value;
		remove => Updater.TimeStepChanged -= value;
	}

	public event Action<IUpdateManager, float, float> DeltaFixedTimeChanged;

	public event Action<IUpdateManager, float, float> MaxVariableTimeChanged;
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

	#region Fixed Time
	[JsonProperty]
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

	public int FixedLag
	{
		get => fixedLag;
		private set => fixedLag = value;
	}

	public int FixedUpdates
	{
		get => fixedUpdates;
		private set => fixedUpdates = value;
	}
	#endregion

	#region Variable Time
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

	[JsonProperty]
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

	public float VariableInterpolation
	{
		get => variableInterpolation;
		private set => variableInterpolation = value;
	}
	#endregion

	#region State
	public bool IsUpdating
	{
		get => Updater.IsUpdating;
		private set => Updater.IsUpdating = value;
	}

	public TimeStep TimeStep
	{
		get => Updater.TimeStep;
		private set => Updater.TimeStep = value;
	}

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

	#region Updates
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
			--FixedUpdates;
			TotalFixedTime += deltaFixedTime;
			UpdateSystems(Engine.Systems.FixedSystems, deltaFixedTime);
		}

		//Variable-time updates
		TotalVariableTime += deltaVariableTime;
		CalculateVariableInterpolation(deltaFixedTime);
		TimeStep = TimeStep.Variable;

		UpdateSystems(Engine.Systems.VariableSystems, deltaVariableTime);

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
		VariableInterpolation = deltaFixedTime <= 0 ? 0 : (TotalVariableTime - TotalFixedTime) / deltaFixedTime;
	}

	private void UpdateSystems(IReadOnlyLinkList<ISystem> systems, float deltaTime)
	{
		for(var current = systems.First; current != null; current = current.Next)
		{
			var system = current.Value;

			UpdateSystem = system;
			system.Update(deltaTime);
			UpdateSystem = null;
		}
	}
	#endregion
}