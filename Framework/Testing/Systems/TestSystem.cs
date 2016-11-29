using Atlas.Engine.Entities;
using Atlas.Engine.Systems;
using Atlas.Testing.Components;
using Atlas.Testing.Families;
using System.Diagnostics;

namespace Atlas.Testing.Systems
{
	class TestSystem:AtlasFamilySystem<TestFamily>
	{
		private uint updateCount = 0;

		public TestSystem()
		{
			Initialize(EntityUpdate);
		}

		protected override void Updating()
		{
			Debug.WriteLine(GetType().Name + "Update " + (++updateCount));
			base.Updating();
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
