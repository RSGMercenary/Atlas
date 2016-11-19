using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Messages
{
	class MessageDispatcher<TSender>:IMessageDispatcher<TSender>
	{
		private Dictionary<MessageType, Signal<IMessage<TSender>>> signals = new Dictionary<MessageType, Signal<IMessage<TSender>>>();
		private int numDispatches = 0;

		public MessageDispatcher()
		{

		}

		public bool AddMessageListener(MessageType type, Action<IMessage<TSender>> listener)
		{
			return AddMessageListener(type, listener, 0);
		}

		public bool AddMessageListener(MessageType type, Action<IMessage<TSender>> listener, int priority = 0)
		{
			if(listener == null)
				return false;

			Signal<IMessage<TSender>> signal;
			if(signals.ContainsKey(type))
			{
				signal = signals[type];
			}
			else
			{
				signal = new Signal<IMessage<TSender>>();
				signals.Add(type, signal);
			}

			return signal.Add(listener, priority) != null;
		}

		public void DispatchMessage(IMessage<TSender> message)
		{
			if(message == null)
				return;
			if(message.MessageType == null)
				return;
			if(!signals.ContainsKey(message.MessageType))
				return;
			++numDispatches;
			Signal<IMessage<TSender>> signal = signals[message.MessageType];
			signal.Dispatch(message);
			--numDispatches;
		}

		public bool HasMessageListener(MessageType type, Action<IMessage<TSender>> listener)
		{
			if(type == null)
				return false;
			if(listener == null)
				return false;
			if(!signals.ContainsKey(type))
				return false;
			Signal<IMessage<TSender>> signal = signals[type];
			return signal.Get(listener) != null;
		}

		public bool RemoveMessageListener(MessageType type, Action<IMessage<TSender>> listener)
		{
			if(listener == null)
				return false;
			if(!signals.ContainsKey(type))
				return false;
			Signal<IMessage<TSender>> signal = signals[type];
			signal.Remove(listener);
			if(signal.NumSlots <= 0)
			{
				signals.Remove(type);
				signal.Dispose();
			}
			return true;
		}

		public bool RemoveMessageListeners()
		{
			bool removed = false;
			foreach(MessageType type in signals.Keys)
			{
				signals[type].Dispose();
				signals.Remove(type);
				removed = true;
			}
			return removed;
		}

		public bool RemoveMessageListeners(MessageType type)
		{
			if(!signals.ContainsKey(type))
				return false;
			Signal<IMessage<TSender>> signal = signals[type];
			signal.Dispose();
		}
	}
}
