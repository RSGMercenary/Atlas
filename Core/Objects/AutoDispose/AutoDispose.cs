using Atlas.Core.Messages;
using System;

namespace Atlas.Core.Objects.AutoDispose
{
	internal class AutoDispose<T> : IAutoDispose
		where T : IAutoDispose, IMessenger<T>
	{
		private readonly T Instance;
		private readonly Func<bool> Condition;
		private bool isAutoDisposable = true;

		public AutoDispose(T instance, Func<bool> condition)
		{
			Instance = instance;
			Condition = condition;
		}

		public bool IsAutoDisposable
		{
			get => isAutoDisposable;
			set
			{
				if(isAutoDisposable == value)
					return;
				var previous = isAutoDisposable;
				isAutoDisposable = value;
				Instance.Message<IAutoDisposeMessage<T>>(new AutoDisposeMessage<T>(value, previous));
				TryAutoDispose();
			}
		}

		public void TryAutoDispose()
		{
			if(isAutoDisposable && Condition())
				Instance.Dispose();
		}
	}
}
