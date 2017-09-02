using Atlas.Engine.Components;
using Atlas.Engine.Messages;
using System;

namespace Atlas.Engine
{
	public interface IEngineObject<T>
		where T : IEngineObject<T>
	{
		IEngine Engine { get; set; }
		EngineObjectState State { get; }

		bool Destroy();

		void AddListener(string type, Action<IMessage<T>> listener, int priority = 0);

		void RemoveListener(string type, Action<IMessage<T>> listener);

		void Message(IMessage<T> message);
	}
}
