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
	public static IEnumerable<T> GetSystems<T>() where T : ISystem => GetSystemTypes<T>().Select(CreateSystem<T>);

	public static IEnumerable<ISystem> GetSystems(Type type) => GetSystemTypes(type).Select(CreateSystem<ISystem>);
	#endregion

	#region System
	public static T GetSystem<T>() where T : ISystem => (T)GetSystem(typeof(T));

	public static ISystem GetSystem(Type type) => CreateSystem<ISystem>(GetSystemTypes(type).Single());
	#endregion

	#region Create
	private static T CreateSystem<T>(Type type) where T : ISystem => (T)Activator.CreateInstance(type);
	#endregion
}