namespace Atlas.ECS.Entities;

public static class IEntityExtensions
{
	public static IEntityBuilder Builder(this IEntity entity)
	{
		return new AtlasEntityBuilder(entity);
	}
}