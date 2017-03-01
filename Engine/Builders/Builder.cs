using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Builders
{
	class Builder<T> : IBuilder<T>
		where T : class
	{
		private Stack<Action> builders = new Stack<Action>();
		private BuildState state = BuildState.Unbuilt;
		private Signal<T, BuildState, BuildState> stateChanged = new Signal<T, BuildState, BuildState>();

		private T target;

		public Builder()
		{

		}

		public Builder(T target)
		{
			this.target = target;
			if(this.target == null)
				this.target = this as T;
		}

		public ISignal<T, BuildState, BuildState> BuildStateChanged { get { return stateChanged; } }

		/// <summary>
		/// Adds a builder Action method to this Builder.
		/// Any sbuclass that needs to be built before the next subclass
		/// can build should add its own builder. This should only be invoked
		/// from within this Component's AddingManager() call, before
		/// base.AddingManager() is called.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public bool AddBuilder(Action builder)
		{
			if(state != BuildState.Unbuilt)
				return false;
			if(builders.Contains(builder))
				return false;
			builders.Push(builder);
			return true;
		}

		public BuildState BuildState
		{
			get
			{
				return state;
			}
			set
			{
				if(state == value)
					return;
				var previous = state;
				state = value;
				stateChanged.Dispatch(target, value, previous);
				if(value == BuildState.Building)
					Built();
			}
		}

		/// <summary>
		/// Adding builder Action methods to this Builder will add it
		/// to a Stack&lt;Action&gt;. Builder methods will be invoked
		/// sequentially, instructing your subclass to begin building.
		/// Once your subclass is finished building, call Built()
		/// so the Builder may proceed to build the next subclass.
		/// </summary>
		public void Built()
		{
			if(state != BuildState.Building)
				return;
			if(builders.Count > 0)
			{
				builders.Pop().Invoke();
			}
			else
			{
				//If we have no more builders to invoke,
				//then building is complete.
				BuildState = BuildState.Built;
			}
		}
	}
}
