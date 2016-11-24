using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Messages
{
	class MessageDispatcher<TSender>:IMessageDispatcher<TSender>
	{
		private Dictionary<string, Signal<IMessage<TSender>>> signals = new Dictionary<string, Signal<IMessage<TSender>>>();
		private int numDispatches = 0;

		public MessageDispatcher()
		{

		}

		public bool AddMessageListener(string type, Action<IMessage<TSender>> listener)
		{
			return AddMessageListener(type, listener, 0);
		}

		public bool AddMessageListener(string type, Action<IMessage<TSender>> listener, int priority = 0)
		{
			if(string.IsNullOrWhiteSpace(type))
				return false;
			if(listener == null)
				return false;
			Signal<IMessage<TSender>> signal;
			if(signals.ContainsKey(type))
			{
				signal = signals[type];
			}
			else
			{
				signal = signals[type] = new Signal<IMessage<TSender>>();
			}
			return signal.Add(listener, priority) != null;
		}

		public void DispatchMessage(IMessage<TSender> message)
		{
			if(message == null)
				return;
			if(message.Type == null)
				return;
			if(!signals.ContainsKey(message.Type))
				return;
			++numDispatches;
			Signal<IMessage<TSender>> signal = signals[message.Type];
			signal.Dispatch(message);
			--numDispatches;
		}

		public bool HasMessageListener(string type, Action<IMessage<TSender>> listener)
		{
			if(string.IsNullOrWhiteSpace(type))
				return false;
			if(listener == null)
				return false;
			if(!signals.ContainsKey(type))
				return false;
			Signal<IMessage<TSender>> signal = signals[type];
			return signal.Get(listener) != null;
		}

		public bool RemoveMessageListener(string type, Action<IMessage<TSender>> listener)
		{
			if(string.IsNullOrWhiteSpace(type))
				return false;
			if(listener == null)
				return false;
			if(!signals.ContainsKey(type))
				return false;
			Signal<IMessage<TSender>> signal = signals[type];
			signal.Remove(listener);
			if(signal.IsEmpty)
			{
				signal.Dispose();
				signals.Remove(type);
			}
			return true;
		}

		public bool RemoveMessageListeners(string type)
		{
			if(string.IsNullOrWhiteSpace(type))
				return false;
			if(!signals.ContainsKey(type))
				return false;
			Signal<IMessage<TSender>> signal = signals[type];
			signal.Dispose();
			signals.Remove(type);
			return true;
		}

		public bool RemoveMessageListeners()
		{
			bool removed = false;
			foreach(string type in signals.Keys)
			{
				Signal<IMessage<TSender>> signal = signals[type];
				signal.Dispose();
				signals.Remove(type);
				removed = true;
			}
			return removed;
		}
	}
}
