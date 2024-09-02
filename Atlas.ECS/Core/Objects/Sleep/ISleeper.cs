using System;

namespace Atlas.Core.Objects.Sleep;

/// <summary>
/// An <see langword="interface"/> providing <see cref="Sleeping"/> and <see cref="IsSleeping"/> properties for sleep instances.
/// </summary>
public interface ISleeper
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="Sleeping"/> has changed.
	/// </summary>
	event Action<ISleeper, int, int> SleepingChanged;

	/// <summary>
	/// The sleeping count of the <see cref="ISleeper"/> instance.
	/// </summary>
	int Sleeping { get; }

	/// <summary>
	/// The sleeping state of the <see cref="ISleeper"/> instance.
	/// </summary>
	bool IsSleeping { get; }

	/// <summary>
	/// Increase/decrease the <see cref="Sleeping"/> count by 1.
	/// </summary>
	void Sleep(bool sleep);
}

/// <summary>
/// An <see langword="interface"/> providing <see cref="Sleeping"/> and <see cref="IsSleeping"/> properties for sleep instances.
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is a <see cref="ISleeper{T}"/>.</typeparam>
public interface ISleeper<out T> : ISleeper where T : ISleeper<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="Sleeping"/> has changed.
	/// </summary>
	new event Action<T, int, int> SleepingChanged;
}