using System;

namespace Atlas.Core.Collections.Hierarchy
{
	[Flags]
	public enum MessageHierarchy
	{
		Self = 1,
		Sibling = 2,
		Parent = 4,
		Child = 8,
		Ancestor = 16,
		Descendent = 32,
		All = Self | Sibling | Parent | Child | Ancestor | Descendent
	}
}
