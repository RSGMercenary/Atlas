using Atlas.Engine.Components;
using Atlas.Engine.Messages;
using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine
{
	public abstract class EngineObject<T> : IEngineObject<T>
		where T : class, IEngineObject<T>
	{
		public static implicit operator bool(EngineObject<T> engineObject)
		{
			return engineObject != null;
		}

		private Dictionary<string, SignalBase> messages = new Dictionary<string, SignalBase>();
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
				Message(new PropertyMessage<T, IEngine>(AtlasMessage.Engine, value, previous));
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
				Message(new PropertyMessage<T, EngineObjectState>(AtlasMessage.State, value, previous));
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

		public void AddListener(string type, Action<IMessage<T>> listener, int priority = 0)
		{
			if(!messages.ContainsKey(type))
				messages.Add(type, new Signal<IMessage<T>>());
			Signal<IMessage<T>> signal = messages[type] as Signal<IMessage<T>>;
			signal.Add(listener, priority);
		}

		public void RemoveListener(string type, Action<IMessage<T>> listener)
		{
			if(!messages.ContainsKey(type))
				return;
			Signal<IMessage<T>> signal = messages[type] as Signal<IMessage<T>>;
			signal.Remove(listener);
			if(signal.HasListeners)
				return;
			messages.Remove(type);
			signal.Dispose();
		}

		virtual public void Message(IMessage<T> message)
		{
			(message as IMessageBase<T>).Target = this as T;
			//Set current target and send event here.
			(message as IMessageBase<T>).CurrentTarget = this as T;
			Messaging(message);
			if(messages.ContainsKey(message.Type))
				(messages[message.Type] as Signal<IMessage<T>>).Dispatch(message);
		}

		virtual protected void Messaging(IMessage<T> message)
		{

		}
	}
}
