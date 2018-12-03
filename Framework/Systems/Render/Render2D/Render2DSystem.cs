using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.Framework.Families;
using Atlas.Framework.Families.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atlas.Framework.Systems.Render
{
	public class Render2DSystem : AtlasSystem, IRender2DSystem
	{
		private IFamily<GameManagerMember> games;
		private IFamily<Render2DMember> renders;

		public Render2DSystem()
		{
			TimeStep = TimeStep.Variable;
		}

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			games = engine.AddFamily<GameManagerMember>();
			renders = engine.AddFamily<Render2DMember>();
		}

		protected override void RemovingEngine(IEngine engine)
		{
			engine.RemoveFamily<GameManagerMember>();
			engine.RemoveFamily<Render2DMember>();
			games = null;
			renders = null;
			base.RemovingEngine(engine);
		}

		protected override void SystemUpdate(float deltaTime)
		{
			foreach(var game in games)
			{
				game.GameManager.SpriteBatch.GraphicsDevice.Clear(game.GameManager.BackgroundColor);
				game.GameManager.SpriteBatch.Begin(SpriteSortMode.BackToFront);
				foreach(var render in renders)
				{
					render.Entity.GlobalMatrix.Decompose(out var scale, out var rotation, out var position);
					game.GameManager.SpriteBatch.Draw(render.Render.Texture,
						  new Vector2(position.X, position.Y),
						  null,
						  render.Render.Color,
						  (float)Math.Atan2(rotation.Z, rotation.W) * 2,
						  render.Render.Origin,
						  new Vector2(scale.X, scale.Y),
						  SpriteEffects.None,
						  1f / (render.Entity.RootIndex + 1));
				}
				game.GameManager.SpriteBatch.End();
			}
		}
	}
}