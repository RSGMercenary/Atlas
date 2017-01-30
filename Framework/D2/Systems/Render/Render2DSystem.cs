using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Systems;
using Atlas.Framework.D2.Families.Render;

namespace Atlas.Framework.D2.Systems.Render
{
	class Render2DSystem:AtlasFamilySystem<RenderFamily>
	{
		public Render2DSystem()
		{
			Initialize(EntityUpdate, true, EntityAdded, EntityRemoved);
		}

		private void EntityUpdate(double deltaTime, IEntity entity)
		{
			//Render stuff here!
		}

		private void EntityAdded(IFamily family, IEntity entity)
		{

		}

		private void EntityRemoved(IFamily family, IEntity entity)
		{

		}
	}
}
