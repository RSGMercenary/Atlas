using System;
using System.Collections.Generic;

namespace Atlas.Families
{
	interface IFamilyType
	{
		IReadOnlyList<Type> Components { get; }
	}
}
