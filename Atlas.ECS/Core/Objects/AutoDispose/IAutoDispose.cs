using System;

namespace Atlas.Core.Objects.AutoDispose;

public interface IAutoDispose : IDisposable
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IsAutoDisposable"/> has changed.
	/// </summary>
	event Action<IAutoDispose, bool> IsAutoDisposableChanged;

	/// <summary>
	/// The state of the <see cref="IAutoDispose"/>.
	/// </summary>
	bool IsAutoDisposable { get; set; }
}

/// <summary>
/// An <see langword="interface"/> providing <see cref="IsAutoDisposable"/> properties for auto dispose instances.
/// <para><see cref="IAutoDispose{T}"/> instances have <see cref="IDisposable.Dispose"/> called when no longer managed.</para>
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is a <see cref="ISleep{T}"/>.</typeparam>
public interface IAutoDispose<out T> : IAutoDispose where T : IAutoDispose<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IAutoDispose.IsAutoDisposable"/> has changed.
	/// </summary>
	new event Action<T, bool> IsAutoDisposableChanged;
}