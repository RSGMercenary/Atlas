using Atlas.Core.Messages;
using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Messages;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasSystem : EngineObject<IReadOnlySystem>, IReadOnlySystem
	{
		private int priority = 0;
		private int sleeping = 0;
		private double fixedTime = -1;
		private bool isUpdating = false;
		private bool updateLock = false;

		public AtlasSystem()
		{

		}

		public sealed override void Dispose()
		{
			if(State != ObjectState.Composed)
				return;
			//Can't destroy System mid-update.
			if(Engine == null || Engine.IsUpdating)
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

		public sealed override IEngine Engine
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
			if(message is IEngineMessage<IReadOnlySystem>)
			{
				var cast = message as IEngineMessage<IReadOnlySystem>;
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

		protected virtual void AddingEngine(IEngine engine) { }

		protected virtual void RemovingEngine(IEngine engine) { }

		public void Update(double deltaTime)
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
			IsUpdating = true;
			Updating(deltaTime);
			IsUpdating = false;
			updateLock = false;
		}

		protected virtual void Updating(double deltaTime) { }

		public bool IsUpdating
		{
			get { return isUpdating; }
			private set
			{
				if(isUpdating == value)
					return;
				var previous = isUpdating;
				isUpdating = value;
				Dispatch<IUpdateMessage<IReadOnlySystem>>(new UpdateMessage<IReadOnlySystem>(this, value, previous));
			}
		}

		public double FixedTime
		{
			get { return fixedTime; }
			protected set
			{
				if(fixedTime == value)
					return;
				var previous = fixedTime;
				fixedTime = value;
				Dispatch<IFixedTimeMessage>(new FixedTimeMessage(this, value, previous));
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
				Dispatch<ISleepMessage<IReadOnlySystem>>(new SleepMessage<IReadOnlySystem>(this, value, previous));
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
				Dispatch<IPriorityMessage>(new PriorityMessage(this, value, previous));
			}
		}
	}
}