using Atlas.Core.Collections.Builder;
using Atlas.Core.Messages;
using System;

namespace Atlas.Core.Objects.Builder
{
	public interface IMessengerBuilder<TBuilder, T> : IBuilder<TBuilder, T>
		where TBuilder : IMessengerBuilder<TBuilder, T>
		where T : class, IMessenger<T>
	{
		TBuilder AddListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>;

		TBuilder AddListener<TMessage>(Action<TMessage> listener, int priority)
			where TMessage : IMessage<T>;

		TBuilder RemoveListener<TMessage>(Action<TMessage> listener)
			where TMessage : IMessage<T>;

		TBuilder RemoveListeners();
	}
}