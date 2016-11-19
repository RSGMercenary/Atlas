using System;

namespace Atlas.Messages
{
	interface IMessageDispatcher<TSender>
	{
		void DispatchMessage(IMessage<TSender> message);
		bool HasMessageListener(string type, Action<IMessage<TSender>> listener);
		bool AddMessageListener(string type, Action<IMessage<TSender>> listener);
		bool AddMessageListener(string type, Action<IMessage<TSender>> listener, int priority = 0);
		bool RemoveMessageListener(string type, Action<IMessage<TSender>> listener);
		bool RemoveMessageListeners(string type);
		bool RemoveMessageListeners();
	}
}
