using Atlas.Core.Collections.LinkList;
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
	private readonly Updater<IUpdateManager> Updater;
	#endregion

	internal UpdateManager(IEngine engine)
	{
		DeltaFixedTime = 1f / 60f;
		MaxVariableTime = 1f / 4f;
		Engine = engine;
		Updater = new(this);
	}

	public IEngine Engine { get; }

	#region Fixed Time
	[JsonProperty]
	public float DeltaFixedTime
	{
		get;
		set
		{
			if(field == value)
				return;
			var previous = field;
			field = value;
			DeltaFixedTimeChanged?.Invoke(this, value, previous);
		}
	}

	public float TotalFixedTime { get; private set; }

	public int FixedLag { get; private set; }

	public int FixedUpdates { get; private set; }
	#endregion

	#region Variable Time
	public float DeltaVariableTime
	{
		get;
		private set
		{
			//Cap delta time to avoid the "spiral of death".
			value = float.Min(value, MaxVariableTime);
			if(field == value)
				return;
			field = value;
		}
	}

	public float TotalVariableTime { get; private set; }

	[JsonProperty]
	public float MaxVariableTime
	{
		get;
		set
		{
			if(field == value)
				return;
			var previous = field;
			field = value;
			MaxVariableTimeChanged?.Invoke(this, value, previous);
		}
	}

	public float VariableInterpolation { get; private set; }
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
		get;
		private set
		{
			if(field == value)
				return;
			//If an event were to ever be put here, do it before the set.
			//Prevents System.Update() from being mis-called.
			field = value;
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
		TotalVariableTime += DeltaVariableTime;
		CalculateVariableInterpolation(deltaFixedTime);
		TimeStep = TimeStep.Variable;

		UpdateSystems(Engine.Systems.VariableSystems, DeltaVariableTime);

		TimeStep = TimeStep.None;
		IsUpdating = false;
	}

	private void CalculateFixedUpdates(float deltaFixedTime)
	{
		if(DeltaVariableTime <= 0)
			return;
		var fixedUpdates = 0;
		var totalFixedTime = TotalFixedTime;
		while(totalFixedTime + deltaFixedTime <= TotalVariableTime + DeltaVariableTime)
		{
			totalFixedTime += deltaFixedTime;
			++fixedUpdates;
		}
		FixedUpdates = fixedUpdates;
	}

	private void CalculateFixedLag()
	{
		//Calculate when fixed-time and variable-time updates weren't 1:1.
		var fixedLag = FixedLag + int.Max(0, FixedUpdates - 1);
		if(FixedUpdates == 1 && fixedLag > 0)
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