using System.Collections.Generic;

namespace Atlas.Core.Collections.Group
{
	public interface IReadOnlyGroup<T> : IReadOnlyList<T>
	{
		bool Contains(T item);
		IEnumerable<T> Forward();
		IEnumerable<T> Backward();
	}

	public interface IGroup<T> : IReadOnlyGroup<T>, IList<T>
	{

	}
}
