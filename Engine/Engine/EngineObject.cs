using Atlas.Engine.Components;
using Atlas.Engine.Messages;
using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Engine
{
	public abstract class EngineObject : IEngineObject
	{
		public static implicit operator bool(EngineObject engineObject)
		{
			return engineObject != null;
		}

		private Dictionary<Type, SignalBase> messages = new Dictionary<Type, SignalBase>();
		private IEngine engine;
		private EngineObjectState state = EngineObjectState.Destroyed;

		public EngineObject()
		{
			Construct();
		}

		~EngineObject()
		{
			Destroy();
		}

		public virtual IEngine Engine
		{
			get { return engine; }
			set
			{
				if(engine == value)
					return;
				var previous = engine;
				engine = value;
				Message<IEngineMessage>(new EngineMessage(value, previous));
			}
		}

		public EngineObjectState State
		{
			get { return state; }
			private set
			{
				if(state == value)
					return;
				var previous = state;
				state = value;
				Message<IEngineStateMessage>(new EngineStateMessage(value, previous));
			}
		}

		protected virtual bool Construct()
		{
			if(state != EngineObjectState.Destroyed)
				return false;
			State = EngineObjectState.Constructing;
			Constructing();
			State = EngineObjectState.Constructed;
			return true;
		}

		public virtual bool Destroy()
		{
			if(state != EngineObjectState.Constructed)
				return false;
			State = EngineObjectState.Destroying;
			Destroying();
			State = EngineObjectState.Destroyed;
			return true;
		}

		/// <summary>
		/// Called when this instance is being constructed. Should not be called manually.
		/// </summary>
		protected virtual void Constructing()
		{

		}

		/// <summary>
		/// Called when this instance is being destroyed. Should not be called manually.
		/// </summary>
		protected virtual void Destroying()
		{

		}

		virtual public void Message<TMessage>(TMessage message)
			where TMessage : IMessage
		{
			((IMessageBase)message).Messenger = this;
			((IMessageBase)message).CurrentMessenger = this;
			//Pass around message internally...
			Messaging(message);
			//...before dispatching externally.
			Type type = typeof(TMessage);
			if(messages.ContainsKey(type))
				((Signal<TMessage>)messages[type]).Dispatch(message);
		}

		virtual protected void Messaging(IMessage message)
		{

		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority = 0)
			where TMessage : IMessage
		{
			Type type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				messages.Add(type, new Signal<TMessage>());
			var signal = (Signal<TMessage>)messages[type];
			signal.Add(listener, priority);
		}

		public void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage
		{
			Type type = typeof(TMessage);
			if(!messages.ContainsKey(type))
				return;
			var signal = messages[type];
			signal.Remove(listener);
			if(signal.HasListeners)
				return;
			messages.Remove(type);
			signal.Dispose();
		}
	}
}
