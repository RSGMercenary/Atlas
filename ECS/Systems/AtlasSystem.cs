using Atlas.ECS.Components;
using Atlas.ECS.Objects;
using Atlas.Framework.Messages;
using Atlas.Framework.Objects;
using System;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasSystem : EngineObject, IReadOnlySystem
	{
		private int priority = 0;
		private int sleeping = 0;
		private UpdatePhase updateState = UpdatePhase.None;
		private bool updateLock = false;

		public AtlasSystem()
		{

		}

		sealed public override void Dispose()
		{
			if(State != ObjectState.Initialized)
				return;
			//Can't destroy System mid-update.
			if(Engine == null || Engine.UpdateState != UpdatePhase.None)
				return;
			Engine = null;
			if(Engine == null)
				base.Dispose();
		}

		protected override void Disposing(bool finalizer)
		{
			Priority = 0;
			Sleeping = 0;
			base.Disposing(finalizer);
		}

		sealed override public IEngine Engine
		{
			get { return base.Engine; }
			set
			{
				if(value != null)
				{
					if(Engine == null && value.HasSystem(this))
					{
						base.Engine = value;
					}
				}
				else
				{
					if(Engine != null && !Engine.HasSystem(this))
					{
						base.Engine = value;
					}
				}
			}
		}

		protected override void Messaging(IMessage message)
		{
			if(message is IEngineMessage)
			{
				var cast = message as IEngineMessage;
				if(cast.PreviousValue != null)
				{
					RemovingEngine(cast.PreviousValue);
				}
				if(cast.CurrentValue != null)
				{
					AddingEngine(cast.CurrentValue);
				}
			}
			base.Messaging(message);
		}

		protected virtual void AddingEngine(IEngine engine)
		{

		}

		protected virtual void RemovingEngine(IEngine engine)
		{

		}

		public void FixedUpdate(double deltaTime)
		{
			Updater(FixedUpdating, deltaTime, UpdatePhase.FixedUpdate);
		}

		public void Update(double deltaTime)
		{
			Updater(Updating, deltaTime, UpdatePhase.Update);
		}

		private void Updater(Action<double> method, double deltaTime, UpdatePhase phase)
		{
			if(IsSleeping)
				return;
			if(Engine == null)
				return;
			if(Engine.CurrentSystem != this)
				return;
			if(updateLock)
				return;
			updateLock = true;
			UpdateState = phase;
			method.Invoke(deltaTime);
			UpdateState = UpdatePhase.None;
			updateLock = false;
		}

		protected virtual void FixedUpdating(double deltaTime)
		{

		}

		protected virtual void Updating(double deltaTime)
		{

		}

		public UpdatePhase UpdateState
		{
			get { return updateState; }
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Message<IUpdatePhaseMessage>(new UpdatePhaseMessage(value, previous));
			}
		}

		public int Sleeping
		{
			get { return sleeping; }
			private set
			{
				if(sleeping == value)
					return;
				int previous = sleeping;
				sleeping = value;
				Message<ISleepMessage>(new SleepMessage(value, previous));
			}
		}

		public bool IsSleeping
		{
			get { return sleeping > 0; }
			set
			{
				if(value)
					++Sleeping;
				else
					--Sleeping;
			}
		}

		public int Priority
		{
			get { return priority; }
			protected set
			{
				if(priority == value)
					return;
				int previous = priority;
				priority = value;
				Message<IPriorityMessage>(new PriorityMessage(value, previous));
			}
		}
	}
}