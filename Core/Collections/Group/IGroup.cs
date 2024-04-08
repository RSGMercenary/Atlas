using System.Collections.Generic;

namespace Atlas.Core.Collections.Group;

public interface IReadOnlyGroup<out T> : IReadOnlyList<T>
{
	IEnumerable<T> Forward();
	IEnumerable<T> Backward();
}

public interface IGroup<T> : IReadOnlyGroup<T>, IList<T> { }