using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;

namespace Atlas.Framework.Components.Transform
{
	interface ITransform:IComponent<ITransform>, IHierarchy<ITransform>
	{
		bool OverrideManagerHierarchy { get; set; }
	}
}
