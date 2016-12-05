using Atlas.Engine.Components;
using Atlas.Engine.Signals;

namespace Atlas.Framework.D2.Components.Render
{
	class Render2D:AtlasComponent, IRender2D
	{
		private int visible = 0;
		private Signal<IRender2D, int, int> visibleChanged = new Signal<IRender2D, int, int>();

		private IRender2D parent;

		public Render2D()
		{

		}

		public bool IsVisible
		{
			get { return visible > 0; }
		}

		public int Visible
		{
			get
			{
				return visible;
			}
			set
			{
				if(visible == value)
					return;
				int previous = visible;
				visible = value;
				visibleChanged.Dispatch(this, visible, previous);
			}
		}

		public ISignal<IRender2D, int, int> VisibleChanged
		{
			get { return visibleChanged; }
		}
	}
}
