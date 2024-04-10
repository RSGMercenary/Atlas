using Atlas.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.ECS.Systems;

public static class SystemGetter
{
	#region Types
	public static IEnumerable<Type> GetSystemTypes<T>() where T : ISystem
	{
		return GetSystemTypes(typeof(T));
	}

	public static IEnumerable<Type> GetSystemTypes(Type type)
	{
		if(type.IsClass)
			return type.ToEnumerable();
		return type.Assembly.GetTypes()
			.Where(t => t.IsAssignableTo(type))
			.Where(t => t.IsClass && !t.IsAbstract);
	}
	#endregion

	#region Systems
	public static IEnumerable<T> GetSystems<T>() where T : ISystem
	{
		return GetSystemTypes(typeof(T)).Cast<T>();
	}

	public static IEnumerable<ISystem> GetSystems(Type type)
	{
		return GetSystemTypes(type).Select(t => (ISystem)Activator.CreateInstance(t));
	}
	#endregion

	#region System
	public static T GetSystem<T>() where T : ISystem
	{
		return (T)GetSystem(typeof(T));
	}

	public static ISystem GetSystem(Type type)
	{
		var types = GetSystemTypes(type);
		if(types.Skip(1).Any())
			throw new InvalidOperationException($"Multiple systems found of type '{type}'.");
		return (ISystem)Activator.CreateInstance(types.First());
	}
	#endregion
}