using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Core.Utilites
{
	public static class Reflect
	{
		public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
		{
			if(type == null)
				yield break;
			foreach(var field in type.GetFields(flags | BindingFlags.DeclaredOnly))
				yield return field;
			foreach(var field in GetAllFields(type.BaseType, flags))
				yield return field;
		}
	}
}