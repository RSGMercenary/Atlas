using System;
using System.Linq;

namespace Atlas.Core.Extensions;

public static class TypeExtensions
{
	public static Type GetInterfaceType<T>(this T instance) => GetInterfaceType<T>(instance.GetType());

	private static Type GetInterfaceType<T>(Type instanceType)
	{
		var type = typeof(T);
		var interfaces = instanceType.GetInterfaces()
			.Except(instanceType.BaseType?.GetInterfaces() ?? Enumerable.Empty<Type>())
			.Where(t => t != type && t.IsAssignableTo(type));

		if(interfaces.Skip(1).Any())
			throw new Exception();
		if(interfaces.Any())
			return interfaces.First();
		if(instanceType.BaseType != null)
			return GetInterfaceType(instanceType.BaseType);

		return null;
	}
}