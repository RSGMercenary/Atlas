using Microsoft.Xna.Framework;

namespace Atlas.Framework.Components.Transform
{
	public class CameraTransform2D : Transform2D
	{
		private readonly Game game;

		public CameraTransform2D(Game game)
		{
			this.game = game;
			game.Window.ClientSizeChanged += WindowClientSizeChanged;
		}

		protected override void Disposing()
		{
			game.Window.ClientSizeChanged -= WindowClientSizeChanged;
			base.Disposing();
		}

		private void WindowClientSizeChanged(object sender, System.EventArgs e)
		{
			SetMatrix();
		}

		protected override Matrix CreateLocalMatrix()
		{
			return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				   Matrix.CreateRotationZ(-Rotation) *
				   Matrix.CreateScale(new Vector3(Scale, 1)) *
				   Matrix.CreateTranslation(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2, 0);
		}
	}
}
