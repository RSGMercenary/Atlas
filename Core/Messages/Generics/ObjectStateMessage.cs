using Atlas.Core.Objects;
using Atlas.ECS.Objects;

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
