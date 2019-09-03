using Atlas.Core.Messages;
using Atlas.ECS.Components.Engine;

namespace Atlas.ECS.Objects
{
	#region Interfaces

	public interface IEngineMessage<out T> : IPropertyMessage<T, IEngine> where T : IObject { }

	#endregion

	#region Classes

	class EngineMessage<T> : PropertyMessage<T, IEngine>, IEngineMessage<T> where T : IObject
	{
		public EngineMessage(IEngine current, IEngine previous) : base(current, previous) { }
	}

	#endregion
}