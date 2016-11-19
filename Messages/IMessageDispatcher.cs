using System;

namespace Atlas.Messages
{
	interface IMessageDispatcher<TSender>
	{
		void DispatchMessage(IMessage<TSender> message);
		bool HasMessageListener(MessageType type, Action<IMessage<TSender>> listener);
		bool AddMessageListener(MessageType type, Action<IMessage<TSender>> listener);
		bool AddMessageListener(MessageType type, Action<IMessage<TSender>> listener, int priority = 0);
		bool RemoveMessageListener(MessageType type, Action<IMessage<TSender>> listener);
		bool RemoveMessageListeners(MessageType type);
		bool RemoveMessageListeners();
	}
}
