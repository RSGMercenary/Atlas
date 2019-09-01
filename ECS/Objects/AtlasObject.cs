using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Objects.Messages;
using System;

namespace Atlas.ECS.Objects
{
	public abstract class AtlasObject<T> : IObject<T>
		where T : class, IObject
	{
		#region Fields

		private IEngine engine;
		private IMessenger<T> messenger;

		#endregion

		#region Construct / Dispose

		public virtual void Dispose()
		{
			Disposing();
		}

		protected virtual void Disposing()
		{
			RemoveListeners();
		}

		#endregion

		#region Engine

		public virtual IEngine Engine
		{
			get => engine;
			set
			{
				if(engine == value)
					return;
				var previous = engine;
				engine = value;
				if(previous != null)
					RemovingEngine(previous);
				if(value != null)
					AddingEngine(value);
				Message<IEngineMessage<T>>(new EngineMessage<T>(value, previous));
			}
		}

		protected virtual void AddingEngine(IEngine engine) { }

		protected virtual void RemovingEngine(IEngine engine) { }

		#endregion

		#region Messages

		protected virtual IMessenger<T> Messenger
		{
			get => messenger = messenger ?? new Messenger<T>(this as T, Messaging);
		}

		public void AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T> => Messenger.AddListener(listener);

		public void AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<T> => Messenger.AddListener(listener);

		public void RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T> => Messenger.RemoveListener(listener);

		public bool RemoveListeners() => Messenger.RemoveListeners();

		public void Message<TMessage>(TMessage message)
			where TMessage : IMessage<T> => Messenger.Message(message);

		protected virtual void Messaging(IMessage<T> message) { }

		#endregion
	}
}