using Atlas.Core.Messages;
using System;

namespace Atlas.ECS.Components.Engine
{
	internal class EngineItem<T> : IEngineItem
		where T : IEngineItem, IMessenger<T>
	{
		private readonly T Instance;
		private readonly Func<IEngine, T, bool> Condition;
		private IEngine engine;

		public EngineItem(T instance, Func<IEngine, T, bool> condition)
		{
			Instance = instance;
			Condition = condition;
		}

		public IEngine Engine
		{
			get => engine;
			set
			{
				if((value != null && Engine == null && Condition(value, Instance)) ||
					(value == null && Engine != null && !Condition(Engine, Instance)))
				{
					var previous = engine;
					engine = value;
					Instance.Message<IEngineMessage<T>>(new EngineMessage<T>(value, previous));
				}
			}
		}
	}
}