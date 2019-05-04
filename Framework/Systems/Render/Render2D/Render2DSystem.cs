using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.Framework.Components.Render;
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
				var batch = game.GameManager.SpriteBatch;
				batch.GraphicsDevice.Clear(game.GameManager.BackgroundColor);
				batch.Begin(blendState: BlendState.NonPremultiplied);
				Draw(batch, game.Entity);
				batch.End();
			}
		}

		private void Draw(SpriteBatch batch, IEntity entity)
		{
			var member = renders.GetMember(entity);
			if(member != null && member.Render.IsVisible)
			{
				if(member.Render is IRenderTexture2D)
					Draw(batch, member, member.Render as IRenderTexture2D);
				else if(member.Render is IRenderText2D)
					Draw(batch, member, member.Render as IRenderText2D);
				else if(member.Render is ITargetRender2D)
				{
					Draw(batch, member, member.Render as ITargetRender2D);
					return;
				}
			}
			foreach(var child in entity)
				Draw(batch, child);
		}

		private void Draw(SpriteBatch batch, Render2DMember member, ITargetRender2D render)
		{
			batch.GraphicsDevice.SetRenderTarget(render.Target);
			foreach(var child in member.Entity)
				Draw(batch, child);
			batch.GraphicsDevice.SetRenderTarget(null);
			Global(member.Transform.Global, out var position, out var rotation, out var scale);
			batch.Draw(
				render.Target,
				position,
				render.Crop,
				render.GlobalColor,
				rotation,
				render.Center,
				scale,
				SpriteEffects.None,
				0);
		}

		private void Draw(SpriteBatch batch, Render2DMember member, IRenderTexture2D render)
		{
			Global(member.Transform.Global, out var position, out var rotation, out var scale);
			batch.Draw(
				render.Texture,
				position,
				render.Crop,
				render.GlobalColor,
				rotation,
				render.Center,
				scale,
				SpriteEffects.None,
				0);
		}

		private void Draw(SpriteBatch batch, Render2DMember member, IRenderText2D render)
		{
			Global(member.Transform.Global, out var position, out var rotation, out var scale);
			batch.DrawString(render.Font,
				render.Text,
				position,
				render.GlobalColor,
				rotation,
				render.Center,
				scale,
				SpriteEffects.None,
				0);
		}

		private void Global(Matrix matrix, out Vector2 pos, out float rot, out Vector2 scl)
		{
			matrix.Decompose(out var scale, out var rotation, out var position);
			pos = new Vector2(position.X, position.Y);
			rot = (float)Math.Atan2(rotation.Z, rotation.W) * 2;
			scl = new Vector2(scale.X, scale.Y);
		}
		/*
		private Color GlobalColor(Render2DMember member)
		{
			var color = member.Render.Color.ToVector4();
			var entity = member.Entity.Parent;
			while(entity != null)
			{
				member = renders.GetMember(entity);
				if(member != null)
					color *= member.Render.Color.ToVector4();
				entity = entity.Parent;
			}
			return new Color(color);
		}*/
	}
}