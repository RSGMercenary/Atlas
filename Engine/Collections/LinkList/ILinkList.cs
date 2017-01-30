using System;
using System.Collections.Generic;

namespace Atlas.Engine.Collections.LinkList
{
    interface ILinkList<T>:IReadOnlyLinkList<T>, ICollection<T>, IDisposable
    {
        void Add(params T[] items);

        void Remove(params T[] items);

        T Add(T item, int index);

        T Remove(int index);

        bool SetIndex(T item, int index);

        new T this[int i] { get; set; }
    }
}
