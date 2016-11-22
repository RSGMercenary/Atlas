using System;
using System.Collections.Generic;

namespace Atlas.Families
{
	class AtlasFamilyType:IFamilyType
	{
		private List<Type> components;

		public AtlasFamilyType(List<Type> components)
		{
			this.components = components != null ? components : new List<Type>();
		}

		public IReadOnlyList<Type> Components
		{
			get
			{
				return components;
			}
		}
	}
}
