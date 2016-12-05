using Atlas.Engine.Components;
using Atlas.Engine.Interfaces;

namespace Atlas.Framework.Components.Transform
{
	interface ITransform2D:IComponent, IHierarchy<ITransform2D, ITransform2DManager>
	{
		//bool OverrideEntityHierarchy { get; set; }
	}
}
