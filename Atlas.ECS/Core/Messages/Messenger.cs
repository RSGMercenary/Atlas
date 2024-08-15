using Atlas.Core.Extensions;
using Atlas.Signals.Signals;
using Atlas.Signals.Slots;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.Core.Messages;

public abstract class Messenger<T> : IMessenger<T>
	where T : class, IMessenger
{
	private readonly Dictionary<Type, SignalBase> messages = new();

	public virtual void Dispose()
	{
		Disposing();
	}

	protected virtual void Disposing()
	{
		RemoveListeners();
	}

	public virtual void Message<TMessage>(TMessage message)
		where TMessage : IMessage<T>
	{
		if(message is IMessage cast)
		{
			cast.Messenger = this;
			cast.CurrentMessenger = this;
		}

		//Pass around message internally...
		Messaging(message);
		//...before dispatching externally.

		if(messages.TryGetValue(typeof(TMessage), out Signal<TMessage> signal))
			signal.Dispatch(message);
	}

	protected virtual void Messaging(IMessage<T> message) { }

	#region Add
	public ISlot<TMessage> AddListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<T>
	{
		return AddListenerSlot(listener, 0);
	}

	public ISlot<TMessage> AddListener<TMessage>(Action<TMessage> listener, int priority)
		where TMessage : IMessage<T>
	{
		return AddListenerSlot(listener, priority);
	}

	protected ISlot<TMessage> AddListenerSlot<TMessage>(Action<TMessage> listener, int priority)
		where TMessage : IMessage<T>
	{
		var type = typeof(TMessage);
		if(!messages.TryGetValue(type, out Signal<TMessage> signal))
		{
			signal = CreateSignal<TMessage>();
			messages.Add(type, signal);
		}
		return signal.Add(listener, priority);
	}
	#endregion

	#region Remove
	public bool RemoveListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<T>
	{
		var type = typeof(TMessage);
		if(messages.TryGetValue(type, out Signal<TMessage> signal))
		{
			signal.Remove(listener);
			if(signal.Slots.Count <= 0)
			{
				messages.Remove(type);
				signal.Dispose();
			}
			return true;
		}
		return false;
	}

	public bool RemoveListeners()
	{
		if(messages.Count <= 0)
			return false;
		foreach(var type in messages.Keys.ToList())
		{
			var signal = messages[type];
			messages.Remove(type);
			signal.Dispose();
		}
		return true;
	}
	#endregion

	public ISlot<TMessage> GetListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<T>
	{
		if(messages.TryGetValue(typeof(TMessage), out Signal<TMessage> signal))
			return signal.Get(listener);
		return null;
	}

	public bool HasListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<T>
	{
		return GetListener(listener) != null;
	}

	protected virtual Signal<TMessage> CreateSignal<TMessage>()
		where TMessage : IMessage<T>
	{
		return new Signal<TMessage>();
	}
}