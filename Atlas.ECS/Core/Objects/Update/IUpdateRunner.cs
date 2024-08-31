using System;

namespace Atlas.Core.Objects.Update;

/// <summary>
/// An <see langword="interface"/> for providing elapsed time updates to an <see cref="IUpdate{T}"/>.
/// </summary>
public interface IUpdateRunner
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IsRunning"/> has changed.
	/// </summary>
	event Action<IUpdateRunner, bool> IsRunningChanged;

	/// <summary>
	/// The running state of the <see cref="IUpdateRunner"/>.
	/// </summary>
	bool IsRunning { get; set; }
}