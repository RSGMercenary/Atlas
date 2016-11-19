using Atlas.Families;
using Atlas.Testing.Components;
using System;
using System.Collections.Generic;

namespace Atlas.Testing.Families
{
	class TestFamily:AtlasFamilyType
	{
		public TestFamily() : base(new List<Type>(new Type[] { typeof(TestComponent) }))
		{

		}
	}
}
