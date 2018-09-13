using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	class AutoDestroyMessage : PropertyMessage<IAutoDestroyObject, bool>, IAutoDestroyMessage
	{
		public AutoDestroyMessage(IAutoDestroyObject messenger, bool current, bool previous) : base(messenger, current, previous)
		{
		}
	}
}
