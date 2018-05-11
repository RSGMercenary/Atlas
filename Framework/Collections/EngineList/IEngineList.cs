using System.Collections.Generic;

namespace Atlas.Framework.Collections.EngineList
{
	public interface IReadOnlyEngineList<T> : IReadOnlyList<T>
	{
		bool Contains(T item);
		IEnumerable<T> Forward();
		IEnumerable<T> Backward();
	}

	public interface IEngineList<T> : IReadOnlyEngineList<T>, IList<T>
	{

	}
}
