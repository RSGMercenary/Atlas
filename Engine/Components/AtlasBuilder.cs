using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Components
{
	class AtlasBuilder:AtlasComponent, IBuilder
	{
		private Stack<Action> builders = new Stack<Action>();
		private bool isBuilding = false;
		private bool isBuilt = false;
		private Signal<IBuilder, bool> isbuildingChanged = new Signal<IBuilder, bool>();
		private Signal<IBuilder, bool> isbuiltChanged = new Signal<IBuilder, bool>();

		public AtlasBuilder()
		{

		}

		public ISignal<IBuilder, bool> IsBuildingChanged { get { return isbuildingChanged; } }
		public ISignal<IBuilder, bool> IsBuiltChanged { get { return isbuiltChanged; } }

		override protected void AddingManager(IEntity entity, int index)
		{
			base.AddingManager(entity, index);
			IsBuilding = true;
		}

		protected override void RemovingManager(IEntity entity, int index)
		{
			IsBuilding = false;
			IsBuilt = false;
			base.RemovingManager(entity, index);
		}

		/// <summary>
		/// Adds a builder Action method to this Builder.
		/// Any sbuclass that needs to be built before the next subclass
		/// can build should add its own builder. This should only be invoked
		/// from within this Component's AddingManager() call, before
		/// base.AddingManager() is called.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		protected bool AddBuilder(Action builder)
		{
			if(isBuilding || isBuilt)
				return false;
			if(builders.Contains(builder))
				return false;
			builders.Push(builder);
			return true;
		}

		public bool IsBuilding
		{
			get
			{
				return isBuilding;
			}
			private set
			{
				if(isBuilding == value)
					return;
				isBuilding = value;
				isbuildingChanged.Dispatch(this, value);
				//Start the build process.
				if(isBuilding)
					Built();
			}
		}

		public bool IsBuilt
		{
			get
			{
				return isBuilt;
			}
			private set
			{
				if(isBuilt == value)
					return;
				//Stop the build process.
				if(value)
					IsBuilding = false;
				isBuilt = value;
				isbuiltChanged.Dispatch(this, value);
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
			if(!isBuilding || isBuilt)
				return;
			if(builders.Count > 0)
			{
				builders.Pop().Invoke();
			}
			else
			{
				//If we have no more builders to invoke,
				//then building is complete.
				IsBuilt = true;
				//Builder is done. Don't need to
				//keep this Component around.
				RemoveManagers();
			}
		}
	}
}
