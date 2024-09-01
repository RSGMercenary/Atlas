using System;

namespace Atlas.Core.Collections.LinkList;

public interface ILinkListIterator<out T> : IReadOnlyLinkList<T>, IDisposable
{
}