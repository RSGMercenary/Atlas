using Atlas.Core.Messages;
using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;
using Atlas.ECS.Systems.Messages;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasSystem : AtlasObject<ISystem>, ISystem
	{
		private int priority = 0;
		private int sleeping = 0;
		private float totalIntervalTime = 0;
		private float deltaIntervalTime = 0;
		private TimeStep timeStep = TimeStep.Variable;
		private TimeStep updateState = TimeStep.None;
		private bool updateLock = false;

		public AtlasSystem()
		{

		}

		public sealed override void Dispose()
		{
			//Can't dispose System mid-update.
			if(Engine != null || updateState != TimeStep.None)
				return;
			base.Dispose();
		}

		protected override void Disposing()
		{
			Priority = 0;
			Sleeping = 0;
			DeltaIntervalTime = 0;
			TotalIntervalTime = 0;
			TimeStep = TimeStep.Variable;
			UpdateState = TimeStep.None;
			base.Disposing();
		}

		public sealed override IEngine Engine
		{
			get { return base.Engine; }
			set
			{
				if(value != null && Engine == null && value.HasSystem(this))
					base.Engine = value;
				else if(value == null && Engine != null && !Engine.HasSystem(this))
					base.Engine = value;
			}
		}

		protected override void AddingEngine(IEngine engine)
		{
			base.AddingEngine(engine);
			SyncTotalIntervalTime();
		}

		protected override void RemovingEngine(IEngine engine)
		{
			TotalIntervalTime = 0;
			base.RemovingEngine(engine);
			Dispose();
		}

		#region Updating

		public void Update(float deltaTime)
		{
			if(IsSleeping)
				return;
			if(Engine?.CurrentSystem != this)
				return;
			if(updateLock)
				return;

			if(deltaIntervalTime > 0)
			{
				deltaTime = deltaIntervalTime;
				if(Engine.TotalVariableTime - totalIntervalTime < deltaIntervalTime)
					return;
				TotalIntervalTime += deltaIntervalTime;
			}

			updateLock = true;
			UpdateState = TimeStep;
			SystemUpdate(deltaTime);
			UpdateState = TimeStep.None;
			updateLock = false;
			if(Engine == null)
				Dispose();
		}

		protected virtual void SystemUpdate(float deltaTime) { }

		#endregion

		#region Sleeping

		public int Sleeping
		{
			get { return sleeping; }
			private set
			{
				if(sleeping == value)
					return;
				int previous = sleeping;
				sleeping = value;
				Message<ISleepMessage<ISystem>>(new SleepMessage<ISystem>(value, previous));
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

		#endregion

		public float DeltaIntervalTime
		{
			get { return deltaIntervalTime; }
			protected set
			{
				if(deltaIntervalTime == value)
					return;
				var previous = deltaIntervalTime;
				deltaIntervalTime = value;
				Message<IIntervalMessage>(new IntervalMessage(value, previous));
				if(Engine != null)
					SyncTotalIntervalTime();
			}
		}

		public float TotalIntervalTime
		{
			get { return totalIntervalTime; }
			private set
			{
				if(totalIntervalTime == value)
					return;
				totalIntervalTime = value;
			}
		}

		/// <summary>
		/// Syncs this System's interval time to match other Systems with the same interval time.
		/// </summary>
		private void SyncTotalIntervalTime()
		{
			if(deltaIntervalTime <= 0)
				return;
			float totalIntervalTime = 0;
			while(totalIntervalTime + deltaIntervalTime <= Engine.TotalVariableTime)
				totalIntervalTime += deltaIntervalTime;
			TotalIntervalTime = totalIntervalTime;
		}

		public TimeStep TimeStep
		{
			get { return timeStep; }
			protected set
			{
				if(timeStep == value)
					return;
				var previous = timeStep;
				timeStep = value;
				Message<IUpdateStateMessage<ISystem>>(new UpdateStateMessage<ISystem>(value, previous));
			}
		}

		/// <summary>
		/// Priority goes -1 -> 1
		/// </summary>
		public int Priority
		{
			get { return priority; }
			set
			{
				if(priority == value)
					return;
				int previous = priority;
				priority = value;
				Message<IPriorityMessage<ISystem>>(new PriorityMessage<ISystem>(value, previous));
			}
		}

		public TimeStep UpdateState
		{
			get { return updateState; }
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Message<IUpdateStateMessage<ISystem>>(new UpdateStateMessage<ISystem>(value, previous));
			}
		}
	}
}