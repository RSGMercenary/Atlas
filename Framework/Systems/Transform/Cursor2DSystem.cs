using Atlas.Core.Objects;
using Atlas.ECS.Systems;
using Atlas.Framework.Components.Transform;
using Atlas.Framework.Families.Transform;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Atlas.Framework.Systems.Transform
{
	public class Cursor2DSystem : AtlasFamilySystem<Cursor2DMember>, ICursor2DSystem
	{
		public Cursor2DSystem()
		{
			TimeStep = TimeStep.Variable;
		}

		protected override void MemberUpdate(float deltaTime, Cursor2DMember member)
		{
			var transform = member.Transform as ICursorTransform2D;
			if(transform.IsDisabled)
				return;
			var state = Mouse.GetState();
			transform.Position = new Vector2(state.X, state.Y);
		}
	}
}