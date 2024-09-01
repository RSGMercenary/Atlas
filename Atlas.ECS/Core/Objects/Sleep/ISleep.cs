using System;

namespace Atlas.Core.Objects.Sleep;

/// <summary>
/// An <see langword="interface"/> providing <see cref="Sleeping"/> and <see cref="IsSleeping"/> properties for sleep instances.
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is a <see cref="ISleep{T}"/>.</typeparam>
public interface ISleep<out T> where T : ISleep<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="Sleeping"/> has changed.
	/// </summary>
	event Action<T, int, int> SleepingChanged;

	/// <summary>
	/// The sleeping state of the <see cref="ISleep{T}"/>.
	/// </summary>
	bool IsSleeping { get; set; }

	/// <summary>
	/// The number of times the <see cref="ISleep{T}"/> was told to sleep.
	/// </summary>
	int Sleeping { get; }
}