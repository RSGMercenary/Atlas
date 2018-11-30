using Atlas.Core.Objects;
using Atlas.ECS.Systems;
using Atlas.Framework.Families.Render;
using Microsoft.Xna.Framework;
using System;

namespace Atlas.Framework.Systems.Render
{
	public class Camera2DSystem : AtlasFamilySystem<Camera2DMember>
	{
		public Camera2DSystem()
		{
			TimeStep = TimeStep.Variable;
			Priority = (int)SystemPriority.Camera2D;
		}

		protected override void MemberUpdate(float deltaTime, Camera2DMember member)
		{
			var world = Matrix.Invert(member.Entity.GlobalMatrix);

			if(member.Camera.Position != null)
			{
				var positionMatrix = member.Camera.Position.GlobalMatrix * world;
				var position = positionMatrix.Translation;
				member.Transform.Position = new Vector2(position.X, position.Y);
			}

			if(member.Camera.Rotation != null)
			{
				var rotationMatrix = member.Camera.Rotation.GlobalMatrix * world;
				rotationMatrix.Decompose(out var scale, out var rotation, out var position);
				member.Transform.Rotation = (float)Math.Atan2(rotation.Z, rotation.W) * 2;
			}
		}
	}
}