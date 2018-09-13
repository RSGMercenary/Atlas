using Atlas.Core.Objects;

namespace Atlas.Core.Messages
{
	class ObjectStateMessage : PropertyMessage<IObject, ObjectState>, IObjectStateMessage
	{
		public ObjectStateMessage(IObject messenger, ObjectState current, ObjectState previous) : base(messenger, current, previous)
		{
		}
	}
}
