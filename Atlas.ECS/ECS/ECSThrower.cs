using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS;

internal static class ECSThrower
{
	internal static void DuplicateName(string current, string next, string parameter)
	{
		throw new ArgumentException($"Can't set '{current}' to '{next}'. The name is already in use.", parameter);
	}

	internal static void DuplicateName(string current, string parameter)
	{
		throw new ArgumentException($"Can't set '{current}'. The name is already in use.", parameter);
	}

	internal static void NotAssignable(Type from, Type to, string parameter)
	{
		throw new ArgumentException($"'{from}' is not assignable to '{to}'.", parameter);
	}

	internal static void NotRootHasEngine()
	{
		throw new InvalidOperationException($"Can't add {nameof(IEngine)} to {nameof(IEntity)} when {nameof(IEntity.IsRoot)} is false.");
	}
}