using System.Collections.Generic;

namespace Atlas.Engine.Collections.LinkList
{
	interface IReadOnlyLinkList<T>:IReadOnlyCollection<T>
	{
		/// <summary>
		/// When attempting to iterate, call <see cref="IterateStart"/> first, and
		/// then <see cref="IterateStop"/> when done.
		/// </summary>
		ILinkListNode<T> First { get; }

		/// <summary>
		/// When attempting to iterate, call <see cref="IterateStart"/> first, and
		/// then <see cref="IterateStop"/> when done.
		/// </summary>
		ILinkListNode<T> Last { get; }

		bool Contains(T item);

		int GetIndex(T item);

		bool IsEmpty { get; }

		void IterateStart();

		void IterateStop();

		int Iterators { get; }

		bool HasIterators { get; }

		T this[int i] { get; }
	}
}
