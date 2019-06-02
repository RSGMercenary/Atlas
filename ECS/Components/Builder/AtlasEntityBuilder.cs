using Atlas.Core.Builders;
using Atlas.Core.Signals;
using Atlas.ECS.Entities;
using System;

namespace Atlas.ECS.Components
{
	public abstract class AtlasEntityBuilder : AtlasComponent, IEntityBuilder
	{
		private Builder<IEntityBuilder> builder;

		public AtlasEntityBuilder()
		{
			builder = new Builder<IEntityBuilder>(this);
			builder.BuildStateChanged.Add(OnBuildStateChanged, int.MinValue);
		}

		public ISignal<IEntityBuilder, BuildState, BuildState> BuildStateChanged
		{
			get { return builder.BuildStateChanged; }
		}

		protected override void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			builder.BuildState = BuildState.Building;
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			builder.BuildState = BuildState.Unbuilt;
			base.RemovingManager(entity, index);
		}

		/// <summary>
		/// Adds a builder Action method to this Builder.
		/// Any sbuclass that needs to be built before the next subclass
		/// can build should add its own builder. This should only be invoked
		/// from within this Component's AddingManager() call, before
		/// base.AddingManager() is called.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		protected bool AddBuilder(Action method)
		{
			return builder.AddBuilder(method);
		}

		public BuildState BuildState
		{
			get
			{
				return builder.BuildState;
			}
		}

		/// <summary>
		/// Adding builder Action methods to this Builder will add it
		/// to a Stack&lt;Action&gt;. Builder methods will be invoked
		/// sequentially, instructing your subclass to begin building.
		/// Once your subclass is finished building, call Built()
		/// so the Builder may proceed to build the next subclass.
		/// </summary>
		protected void Built()
		{
			builder.Built();
		}

		private void OnBuildStateChanged(IEntityBuilder builder, BuildState next, BuildState previous)
		{
			if(next == BuildState.Built)
				RemoveManagers();
		}
	}
}