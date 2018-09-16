using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class ObjectStateMessage<TMessenger> : PropertyMessage<TMessenger, ObjectState>, IObjectStateMessage<TMessenger>
		where TMessenger : IObject
	{
		public ObjectStateMessage(TMessenger messenger, ObjectState current, ObjectState previous) : base(messenger, current, previous)
		{
		}
	}
}
