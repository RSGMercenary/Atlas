using System;

namespace Atlas.Interfaces.Messages
{
	interface IMessageSignal<Sender> where Sender:class
	{
		void Dispatch(IMessage<Sender> message);

		void AddListener(Action<IMessage<Sender>> listener);
		void AddListener(Action<IMessage<Sender>> listener, int priority = 0);

		void RemoveListener(Action<IMessage<Sender>> listener);
	}
}
