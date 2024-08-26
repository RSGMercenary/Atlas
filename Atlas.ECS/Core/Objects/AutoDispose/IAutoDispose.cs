using System;

namespace Atlas.Core.Objects.AutoDispose;

public interface IAutoDispose<T>
{
	event Action<T, bool, bool> IsAutoDisposableChanged;

	bool IsAutoDisposable { get; set; }
}