using Atlas.Framework.Objects;

namespace Atlas.Framework.Messages
{
	class ObjectStateMessage : PropertyMessage<IObject, ObjectState>, IObjectStateMessage
	{
		public ObjectStateMessage(ObjectState current, ObjectState previous) : base(current, previous)
		{
		}
	}
}
