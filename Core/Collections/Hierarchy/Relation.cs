using System;

namespace Atlas.Core.Collections.Hierarchy
{
	[Flags]
	public enum Relation
	{
		Self = 1,
		Sibling = 2,
		Parent = 4,
		Child = 8,
		Ancestor = 16,
		Descendent = 32,
		Root = 64,
		Up = Parent | Ancestor | Root,
		Down = Child | Descendent,
		Side = Self | Sibling,
		All = Self | Sibling | Parent | Child | Ancestor | Descendent | Root
	}
}