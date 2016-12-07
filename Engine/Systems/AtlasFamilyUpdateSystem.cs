namespace Atlas.Engine.Systems
{
	abstract class AtlasFamilyUpdateSystem<TFamilyType>:AtlasFamilySystem<TFamilyType>
	{
		public AtlasFamilyUpdateSystem()
		{

		}

		override protected void Updating(double deltaTime)
		{
			FamilyUpdate(deltaTime);
		}
	}
}