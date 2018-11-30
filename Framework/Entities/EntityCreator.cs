using Atlas.ECS.Components;
using Atlas.ECS.Entities;
using Atlas.Framework.Components.Render;
using Atlas.Framework.Components.Transform;
using Microsoft.Xna.Framework;

namespace Atlas.Framework.Entities
{
	public static class EntityCreator
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
		/// </para>
		/// </summary>
		/// <returns></returns>
		public static IEngine InitializeMonoGame(Game game)
		{
			//Initial Game instance setup.
			game.Window.AllowUserResizing = true;
			game.Content.RootDirectory = "Content";
			//Atlas handles fixed timestep. Let MonoGame run as fast as possible.
			game.IsFixedTimeStep = false;
			game.IsMouseVisible = true;

			var root = new AtlasEntity(true);
			root.AddComponent<IEngine>(new AtlasEngine());
			root.AddComponent<IRenderer2D>(new Renderer2D(game.GraphicsDevice));
			root.AddComponent<ITransform2D>(new Transform2D());

			//Back GUI. Drawn under the world. (X, Y) coordinates are at (0, 0).
			//Not sure what it could be used for, but... I set it up.
			var back = root.AddChild("Back GUI", true);
			back.AddComponent<ITransform2D>(new Transform2D());

			/*
			Where the magic happens. The world is positioned and optionally rotated
			based on the Target IEntity the camera is looking at. The camera's
			Transform scale determines how zoomed in the world is.
			*/
			var world = root.AddChild("World", true);
			world.AddComponent<ITransform2D>(new CameraTransform2D(game));
			world.AddComponent<ICamera2D>(new Camera2D());

			//Front GUI. Drawn over the world. (X, Y) coordinates are at (0, 0).
			//Use this to add layers for HUDs, menus, popups, etc.
			var front = root.AddChild("Front GUI", true);
			front.AddComponent<ITransform2D>(new Transform2D());

			return root.GetComponent<IEngine>();
		}
	}
}