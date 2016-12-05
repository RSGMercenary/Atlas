using Atlas.Engine.Signals;

namespace Atlas.Framework.D2.Components.Render
{
	interface IRender2D
	{
		bool IsVisible { get; }
		int Visible { get; set; }
		ISignal<IRender2D, int, int> VisibleChanged { get; }
	}
}
