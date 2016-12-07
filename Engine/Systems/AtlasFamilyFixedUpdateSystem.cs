namespace Atlas.Engine.Systems
{
	abstract class AtlasFamilyFixedUpdateSystem<TFamilyType>:AtlasFamilyUpdateSystem<TFamilyType>
	{
		public AtlasFamilyFixedUpdateSystem()
		{

		}

		override protected void FixedUpdating(double deltaTime)
		{
			FamilyUpdate(deltaTime);
		}
	}
}