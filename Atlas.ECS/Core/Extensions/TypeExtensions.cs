using System;
using System.Linq;

namespace Atlas.Core.Extensions;

public static class TypeExtensions
{
	public static Type GetInterfaceType<T>(this T instance, bool inclusive = false) => GetInterfaceType<T>(instance.GetType(), inclusive);

	private static Type GetInterfaceType<T>(Type instanceType, bool inclusive = false)
	{
		var type = typeof(T);
		var interfaces = instanceType.GetInterfaces()
			.Except(instanceType.BaseType?.GetInterfaces() ?? Enumerable.Empty<Type>())
			.Where(i => i == type == inclusive && i.IsAssignableTo(type));

		if(interfaces.Skip(1).Any())
			throw new ArgumentException();
		if(interfaces.Any())
			return interfaces.First();
		if(instanceType.BaseType != null)
			return GetInterfaceType<T>(instanceType.BaseType, inclusive);

		return null;
	}
}