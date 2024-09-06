using System;

namespace Atlas.Core.Objects.AutoDispose;

public interface IAutoDisposer : IDisposable
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="AutoDispose"/> has changed.
	/// </summary>
	event Action<IAutoDisposer, bool> AutoDisposeChanged;

	/// <summary>
	/// The state of the <see cref="IAutoDisposer"/>.
	/// </summary>
	bool AutoDispose { get; set; }
}

/// <summary>
/// An <see langword="interface"/> providing <see cref="IsAutoDisposable"/> properties for auto dispose instances.
/// <para><see cref="IAutoDisposer{T}"/> instances have <see cref="IDisposable.Dispose"/> called when no longer managed.</para>
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is a <see cref="ISleep{T}"/>.</typeparam>
public interface IAutoDisposer<out T> : IAutoDisposer where T : IAutoDisposer
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IAutoDisposer.AutoDispose"/> has changed.
	/// </summary>
	new event Action<T, bool> AutoDisposeChanged;
}