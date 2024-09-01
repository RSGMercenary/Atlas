namespace Atlas.ECS.Entities;

public static class EntityExtensions
{
	public static IEntityBuilder Builder(this IEntity entity)
	{
		return new AtlasEntityBuilder(entity);
	}
}