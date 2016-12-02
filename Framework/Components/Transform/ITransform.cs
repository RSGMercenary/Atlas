using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;

namespace Atlas.Framework.Components.Transform
{
	interface ITransform2D:IComponent<ITransform2D>, IHierarchy<ITransform2D, ITransform2D>
	{
		bool OverrideEntityHierarchy { get; set; }
	}
}
