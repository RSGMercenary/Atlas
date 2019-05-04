using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.Framework.Components.Transform;
using Atlas.Framework.Families;
using Atlas.Framework.Families.Transform;
using Microsoft.Xna.Framework;
using System;

namespace Atlas.Framework.Systems.Transform
{
	public class Camera2DSystem : AtlasSystem, ICamera2DSystem
	{
		private IFamily<GameManagerMember> games;
		private IFamily<Camera2DMember> cameras;

		public Camera2DSystem()
		{
			TimeStep = TimeStep.Variable;
		}

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			games = engine.AddFamily<GameManagerMember>();
			cameras = engine.AddFamily<Camera2DMember>();
		}

		protected override void RemovingEngine(IEngine engine)
		{
			engine.RemoveFamily<GameManagerMember>();
			engine.RemoveFamily<Camera2DMember>();
			games = null;
			cameras = null;
			base.RemovingEngine(engine);
		}

		protected override void SystemUpdate(float deltaTime)
		{
			foreach(var game in games)
			{
				var viewport = game.GameManager.Game.GraphicsDevice.Viewport;
				var center = new Vector2(viewport.Width / 2, viewport.Height / 2);

				foreach(var camera in cameras)
				{
					var world = Matrix.Invert(camera.Transform.Global);

					var position = camera.Transform.Position;
					var rotation = camera.Transform.Rotation;
					var scale = camera.Transform.Scale;

					if(camera.Camera.FollowPosition != null)
					{
						var positionMatrix = camera.Camera.FollowPosition.Global * world;
						var translation = positionMatrix.Translation;
						position = new Vector2(translation.X, translation.Y);
					}

					if(camera.Camera.FollowRotation != null)
					{
						var rotationMatrix = camera.Camera.FollowRotation.Global * world;
						rotationMatrix.Decompose(out var scl, out var rot, out var pos);
						rotation = (float)Math.Atan2(rot.Z, rot.W) * 2;
					}

					(camera.Transform as ICameraTransform2D).Set(position, rotation, scale, center);
				}
			}
		}
	}
}