using System;

namespace Atlas.Core.Objects.Update;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IsUpdating"/> and <see cref="TimeStep"/> properties for update instances.
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is an <see cref="IUpdater{T}"/>.</typeparam>
public interface IUpdater<out T> where T : IUpdater<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IsUpdating"/> has changed.
	/// </summary>
	event Action<T, bool> IsUpdatingChanged;

	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="TimeStep"/> has changed.
	/// </summary>
	event Action<T, TimeStep, TimeStep> TimeStepChanged;

	/// <summary>
	/// The update state of the <see cref="IUpdater{T}"/>.
	/// </summary>
	bool IsUpdating { get; }

	/// <summary>
	/// The <see cref="Update.TimeStep"/> of the <see cref="IUpdater{T}"/>.
	/// </summary>
	TimeStep TimeStep { get; }
}