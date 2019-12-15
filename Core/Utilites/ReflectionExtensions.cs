using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Core.Utilites
{
	public static class ReflectionExtensions
	{
		public static IEnumerable<FieldInfo> FindFields(this Type type, BindingFlags flags)
		{
			if(type == null)
				yield break;
			foreach(var field in type.GetFields(flags | BindingFlags.DeclaredOnly))
				yield return field;
			foreach(var field in FindFields(type.BaseType, flags))
				yield return field;
		}

		public static FieldInfo FindField(this Type type, string name, BindingFlags flags)
		{
			return type != null ? type.GetField(name, flags | BindingFlags.DeclaredOnly) ?? FindField(type.BaseType, name, flags) : null;
		}
	}
}