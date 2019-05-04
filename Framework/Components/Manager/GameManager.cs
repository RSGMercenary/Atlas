using Atlas.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Atlas.Framework.Components.Manager
{
	public class GameManager : AtlasComponent, IGameManager
	{
		public Game Game { get; private set; }
		public Color BackgroundColor { get; set; } = Color.Black;
		public SpriteBatch SpriteBatch { get; private set; }

		public GameManager(Game game)
		{
			Game = game;
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			game.Content.RootDirectory = "Content";

			//Window
			game.Window.AllowUserResizing = true;

			var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			var graphics = GraphicsDeviceManager;

			//60 for the top title bar of the window.
			graphics.PreferredBackBufferWidth = display.Width;
			graphics.PreferredBackBufferHeight = display.Height - 60;
			graphics.ApplyChanges();

			game.IsMouseVisible = false;

			//Atlas handles fixed timestep. Let MonoGame run as fast as possible.
			//Turn fixed timestep off and put the max time at something not hittable.
			game.IsFixedTimeStep = false;
			game.MaxElapsedTime = TimeSpan.FromSeconds(500);
		}

		public GraphicsDeviceManager GraphicsDeviceManager
		{
			get { return Game.Services.GetService<IGraphicsDeviceManager>() as GraphicsDeviceManager; }
		}
	}
}