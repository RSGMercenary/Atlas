namespace Atlas.ECS.Components.Engine
{
	public static class EngineObject
	{
		public static void SetEngine<T>(IEngineObject<T> obj, ref IEngine current, IEngine next) where T : IEngineObject<T>
		{
			var previous = current;
			current = next;
			obj.Message<IEngineMessage<T>>(new EngineMessage<T>(current, previous));
		}
	}
}