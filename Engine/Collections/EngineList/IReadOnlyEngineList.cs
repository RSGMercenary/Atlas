using System.Collections.Generic;

namespace Atlas.Engine.Collections.EngineList
{
	public interface IReadOnlyEngineList<T> : IReadOnlyList<T>
	{
		bool Contains(T item);
		IEnumerable<T> Forward();
		IEnumerable<T> Backward();
	}
}
