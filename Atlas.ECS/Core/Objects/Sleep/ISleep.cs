using System;

namespace Atlas.Core.Objects.Sleep;

/// <summary>
/// An <see langword="interface"/> providing <see cref="Sleeping"/> and <see cref="IsSleeping"/> properties for sleep instances.
/// </summary>
public interface ISleep
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="Sleeping"/> has changed.
	/// </summary>
	event Action<ISleep, int, int> SleepingChanged;

	/// <summary>
	/// The sleeping state of the <see cref="ISleep"/>.
	/// </summary>
	bool IsSleeping { get; set; }

	/// <summary>
	/// The number of times the <see cref="ISleep"/> was told to sleep.
	/// </summary>
	int Sleeping { get; }
}

/// <summary>
/// An <see langword="interface"/> providing <see cref="Sleeping"/> and <see cref="IsSleeping"/> properties for sleep instances.
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is a <see cref="ISleep{T}"/>.</typeparam>
public interface ISleep<out T> : ISleep where T : ISleep<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="Sleeping"/> has changed.
	/// </summary>
	new event Action<T, int, int> SleepingChanged;
}