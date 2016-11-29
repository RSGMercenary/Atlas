using Atlas.Engine.Entities;
using Atlas.Engine.Systems;
using Atlas.Testing.Components;
using Atlas.Testing.Families;
using System.Diagnostics;

namespace Atlas.Testing.Systems
{
	class TestSystem:AtlasFamilySystem<TestFamily>
	{
		public TestSystem()
		{
			Initialize(EntityUpdate);
		}

		private void EntityUpdate(IEntity entity)
		{
			Debug.WriteLine(entity.GlobalName);
			if(entity.GlobalName == "0-3-0")
			{
				entity.RemoveComponent<TestComponent>();
			}
		}

	}
}
