using Atlas.Core.Objects;
using System.Collections.Generic;

namespace Atlas.Core.Utilites
{
	public static class Priority
	{
		public static void Prioritize<T>(IList<T> items, T item)
			where T : IReadOnlyPriority
		{
			items.Remove(item);
			for(var index = items.Count; index > 0; --index)
			{
				if(items[index - 1].Priority <= item.Priority)
				{
					items.Insert(index, item);
					return;
				}
			}
			items.Insert(0, item);
		}
	}
}
