using Atlas.Core.Messages;

namespace Atlas.ECS.Components.Engine
{
	internal class EngineObject<T> : IEngineObject
		where T : IEngineObject, IMessenger<T>
	{
		private readonly T Instance;
		private IEngine engine;

		public EngineObject(T instance)
		{
			Instance = instance;
		}

		public IEngine Engine
		{
			get => engine;
			set
			{
				if((value != null && Engine == null && value.HasObject(Instance)) ||
					(value == null && Engine != null && !Engine.HasObject(Instance)))
				{
					var previous = engine;
					engine = value;
					Instance.Message<IEngineMessage<T>>(new EngineMessage<T>(value, previous));
				}
			}
		}
	}
}