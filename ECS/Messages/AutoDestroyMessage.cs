using Atlas.ECS.Objects;

namespace Atlas.Framework.Messages
{
	class AutoDestroyMessage : PropertyMessage<IAutoDestroyObject, bool>, IAutoDestroyMessage
	{
		public AutoDestroyMessage(bool current, bool previous) : base(current, previous)
		{
		}
	}
}
