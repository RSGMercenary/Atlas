using Atlas.Engine.Entities;

namespace Atlas.Engine.Systems
{
	abstract class AtlasFamilyFixedUpdateSystem<TFamilyType>:AtlasFamilyUpdateSystem<TFamilyType>
	{
		public AtlasFamilyFixedUpdateSystem()
		{

		}

		override protected void FixedUpdating(double deltaTime)
		{
			if(EntityUpdate == null)
				return;
			if(Family == null)
				return;
			for(var current = Family.Entities.First; current != null; current = current.Next)
			{
				IEntity entity = current.Value;
				if(UpdateEntitiesSleeping || entity.IsSleeping)
					EntityUpdate(deltaTime, entity);
			}
		}
	}
}