using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.Framework.Components.Manager;
using Atlas.Framework.Components.Render;
using Atlas.Framework.Components.Transform;
using Atlas.Framework.Systems.Render;
using Atlas.Framework.Systems.Transform;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas.Framework
{
	public static class AtlasGame
	{
		/// <summary>
		/// <para>
		/// A call-once method for initializing MonoGame, creating an <see cref="IEngine"/> instance
		/// and creating an <see cref="IEntity"/> hierarchy consisting of:
		/// </para>
		/// <para>
		/// 1. IEngine
		/// 2. "Root" IEntity
		/// 3. "Root"->"Back GUI" IEntity
		/// 4. "Root"->"World" IEntity
		/// 5. "Root"->"Front GUI" IEntity
		/// 6. "Root"->"Cursor" Entity
		/// </para>
		/// </summary>
		/// <returns></returns>
		public static AtlasEngine Create(Game game)
		{
			var root = new AtlasEntity(true);
			root.AddComponent<IEngine>(new AtlasEngine());
			root.AddComponent<IGameManager>(new GameManager(game));
			root.AddComponent<ITransform2D>(new Transform2D());
			root.AddComponent<ISystemManager>(new SystemManager(
				typeof(ICamera2DSystem),
				typeof(ICursor2DSystem),
				typeof(IRender2DSystem)));

			//Back GUI. Drawn under the world. (X, Y) coordinates are at (0, 0).
			//Not sure what it could be used for, but... I set it up.
			var back = root.AddChild("Back GUI", true);
			back.AddComponent<ITransform2D>(new Transform2D());

			//Where the magic happens. The camera is positioned and optionally rotated
			//based on what IEntity the camera is looking at. The camera's
			//Transform scale determines how zoomed in the world is.
			var camera = root.AddChild("Camera", true);

			//Custom Transform that lets you use the Transform in world coordinates
			//but calculates the Entity Matrix to compensate.
			camera.AddComponent<ITransform2D>(new CameraTransform2D());
			camera.AddComponent<ICamera2D>(new Camera2D());

			//Front GUI. Drawn over the world. (X, Y) coordinates are at (0, 0).
			//Use this to add layers for HUDs, menus, popups, etc.
			var front = root.AddChild("Front GUI", true);
			front.AddComponent<ITransform2D>(new Transform2D());

			//The cursor. Goes in front of all other objects by default, but can also be placed
			//anywhere in the hierarchy to have it draw under/over other things.
			var cursor = root.AddChild("Cursor", true);

			//Custom Transform that lets you use the Transform in mouse/screen coordinates
			//when actively following, but calculates the Entity Matrix to compensate.
			cursor.AddComponent<ITransform2D>(new CursorTransform2D());
			cursor.AddComponent<ICursor2D>(new Cursor2D());
			cursor.AddComponent<IRender2D>(new Render2D(new Texture2D(game.GraphicsDevice, 5, 5), Color.White));

			return root.GetComponent<IEngine, AtlasEngine>();
		}
	}
}