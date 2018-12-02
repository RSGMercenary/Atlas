using System;

namespace Atlas.ECS.Entities
{
	[Flags]
	public enum Hierarchy
	{
		Self = 1,
		Sibling = 2,
		Parent = 4,
		Child = 8,
		Ancestor = 16,
		Descendent = 32,
		Root = 64,
		All = Self | Sibling | Parent | Child | Ancestor | Descendent | Root
	}
}
