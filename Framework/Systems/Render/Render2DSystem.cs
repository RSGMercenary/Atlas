using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.Framework.Families.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atlas.Framework.Systems.Render
{
	public class Render2DSystem : AtlasSystem
	{
		private IReadOnlyFamily<Renderer2DMember> renderers;
		private IReadOnlyFamily<Render2DMember> renders;

		public Render2DSystem()
		{
			TimeStep = TimeStep.Variable;
			Priority = (int)SystemPriority.Render2D;
		}

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			renderers = engine.AddFamily<Renderer2DMember>();
			renders = engine.AddFamily<Render2DMember>();
		}

		protected override void RemovingEngine(IEngine engine)
		{
			engine.AddFamily<Renderer2DMember>();
			engine.AddFamily<Render2DMember>();
			renderers = null;
			renders = null;
			base.RemovingEngine(engine);
		}

		protected override void SystemUpdate(float deltaTime)
		{
			var entityCount = Engine.Entities.Count;
			//There should only be 1 Renderer2D at all times.
			foreach(var renderer in renderers)
			{
				renderer.Renderer.Renderer.GraphicsDevice.Clear(renderer.Renderer.Background);
				renderer.Renderer.Renderer.Begin(SpriteSortMode.FrontToBack);
				foreach(var render in renders)
				{
					render.Entity.GlobalMatrix.Decompose(out var scale, out var rotation, out var position);
					renderer.Renderer.Renderer.Draw(render.Render.Texture,
						  new Vector2(position.X, position.Y),
						  null,
						  Color.White,
						  (float)Math.Atan2(rotation.Z, rotation.W) * 2,
						  render.Render.Origin,
						  new Vector2(scale.X, scale.Y),
						  SpriteEffects.None,
						  render.Entity.RootIndex / entityCount);
				}
				renderer.Renderer.Renderer.End();
			}
		}
	}
}