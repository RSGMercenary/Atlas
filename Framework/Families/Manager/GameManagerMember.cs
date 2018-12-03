using Atlas.ECS.Families;
using Atlas.Framework.Components.Manager;

namespace Atlas.Framework.Families
{
	public class GameManagerMember : AtlasFamilyMember
	{
		private IGameManager gameManager;
		public IGameManager GameManager { get { return gameManager; } }
	}
}
