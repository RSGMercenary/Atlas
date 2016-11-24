using Atlas.Engine.Components;
using Atlas.Interfaces;

namespace Atlas.Framework.Components.Transform
{
	interface ITransform:IComponent<ITransform>, IHierarchy<ITransform>
	{
		bool OverrideManagerHierarchy { get; set; }
	}
}
