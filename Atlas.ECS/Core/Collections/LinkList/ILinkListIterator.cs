using System;

namespace Atlas.Core.Collections.LinkList;

public interface ILinkListIterator<T> : IReadOnlyLinkList<T>, IDisposable
{
}