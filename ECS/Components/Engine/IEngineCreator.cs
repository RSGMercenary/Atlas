using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components
{
	public interface IEngineCreator
	{
		IFamily<TFamilyMember> CreateFamily<TFamilyMember>()
			where TFamilyMember : IFamilyMember, new();

		ISystem CreateSystem(Type type);
	}
}
