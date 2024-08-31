using System;

namespace Atlas.Core.Objects.AutoDispose;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IsAutoDisposable"/> properties for auto dispose instances.
/// <para><see cref="IAutoDispose{T}"/> instances have <see cref="IDisposable.Dispose"/> called when no longer managed.</para>
/// </summary>
/// <typeparam name="T">A generic <see cref="Type"/> that is a <see cref="ISleep{T}"/>.</typeparam>
public interface IAutoDispose<out T> where T : IAutoDispose<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="IsAutoDisposable"/> has changed.
	/// </summary>
	event Action<T, bool, bool> IsAutoDisposableChanged;

	/// <summary>
	/// The state of the <see cref="IAutoDispose{T}"/>.
	/// </summary>
	bool IsAutoDisposable { get; set; }
}