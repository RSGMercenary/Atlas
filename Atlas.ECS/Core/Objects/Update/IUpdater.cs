using System;

namespace Atlas.Core.Objects.Update;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IsUpdating"/> and <see cref="TimeStep"/> properties for update instances.
/// </summary>
public interface IUpdater
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IsUpdating"/> has changed.
	/// </summary>
	event Action<IUpdater, bool> IsUpdatingChanged;

	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="TimeStep"/> has changed.
	/// </summary>
	event Action<IUpdater, TimeStep, TimeStep> TimeStepChanged;

	/// <summary>
	/// The update state of the <see cref="IUpdater"/>.
	/// </summary>
	bool IsUpdating { get; }

	/// <summary>
	/// The <see cref="Update.TimeStep"/> of the <see cref="IUpdater"/>.
	/// </summary>
	TimeStep TimeStep { get; }
}

/// <summary>
/// An <see langword="interface"/> providing <see cref="IsUpdating"/> and <see cref="TimeStep"/> properties for update instances.
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is an <see cref="IUpdater{T}"/>.</typeparam>
public interface IUpdater<out T> : IUpdater where T : IUpdater
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IsUpdating"/> has changed.
	/// </summary>
	new event Action<T, bool> IsUpdatingChanged;

	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="TimeStep"/> has changed.
	/// </summary>
	new event Action<T, TimeStep, TimeStep> TimeStepChanged;
}