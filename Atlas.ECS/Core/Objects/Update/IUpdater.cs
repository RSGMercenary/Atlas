using System;

namespace Atlas.Core.Objects.Update;

public interface IUpdater<T> where T : IUpdater<T>
{
	event Action<T, bool> IsUpdatingChanged;
	event Action<T, TimeStep, TimeStep> TimeStepChanged;

	bool IsUpdating { get; }

	TimeStep TimeStep { get; }
}