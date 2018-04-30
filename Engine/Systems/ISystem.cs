namespace Atlas.Engine.Systems
{
	public interface ISystem : ISystemBase
	{
		void FixedUpdate(double deltaTime);
		void Update(double deltaTime);
	}
}
