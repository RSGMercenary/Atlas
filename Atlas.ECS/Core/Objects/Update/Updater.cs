using System;

namespace Atlas.Core.Objects.Update;

internal sealed class Updater<T> : IUpdater<T>, IDisposable
	where T : class, IUpdater
{
	#region Events
	public event Action<T, bool> IsUpdatingChanged;

	event Action<IUpdater, bool> IUpdater.IsUpdatingChanged
	{
		add => IsUpdatingChanged += value;
		remove => IsUpdatingChanged -= value;
	}

	public event Action<T, TimeStep, TimeStep> TimeStepChanged;

	event Action<IUpdater, TimeStep, TimeStep> IUpdater.TimeStepChanged
	{
		add => TimeStepChanged += value;
		remove => TimeStepChanged -= value;
	}
	#endregion

	#region Fields
	private readonly T Instance;
	private TimeStep timeStep = TimeStep.None;
	#endregion

	public Updater(T instance)
	{
		Instance = instance;
	}

	public void Dispose()
	{
		IsUpdatingChanged = null;
		IsUpdating = false;

		TimeStepChanged = null;
		timeStep = TimeStep.None;
	}

	public bool IsUpdating
	{
		get => field;
		set
		{
			if(field == value)
				return;
			field = value;
			IsUpdatingChanged?.Invoke(Instance, value);
		}
	}

	public void Assert()
	{
		if(IsUpdating)
			throw new InvalidOperationException("Update is already running, and can't be run again.");
	}

	public TimeStep TimeStep
	{
		get => timeStep;
		set
		{
			if(timeStep == value)
				return;
			var previous = timeStep;
			timeStep = value;
			TimeStepChanged?.Invoke(Instance, value, previous);
		}
	}
}