using Atlas.Core.Messages;
using Atlas.Core.Objects.Priority;
using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Objects;
using System;

namespace Atlas.ECS.Systems
{
	public abstract class AtlasSystem : Messenger<ISystem>, ISystem
	{
		#region Fields

		private IEngine engine;
		private int priority = 0;
		private int sleeping = 0;
		private float totalIntervalTime = 0;
		private float deltaIntervalTime = 0;
		private TimeStep timeStep = TimeStep.Variable;
		private TimeStep updateState = TimeStep.None;
		private bool updateLock = false;

		#endregion

		#region Construct / Dispose
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

		#endregion

		#region Engine

		public IEngine Engine
		{
			get => engine;
			set
			{
				if(value != null && Engine == null && value.HasSystem(this))
				{
					var previous = engine;
					engine = value;
					AddingEngine(value);
					Message<IEngineMessage<ISystem>>(new EngineMessage<ISystem>(value, previous));
				}
				else if(value == null && Engine != null && !Engine.HasSystem(this))
				{
					var previous = engine;
					engine = value;
					RemovingEngine(previous);
					Message<IEngineMessage<ISystem>>(new EngineMessage<ISystem>(value, previous));
					Dispose();
				}
			}
		}

		protected virtual void AddingEngine(IEngine engine)
		{
			SyncTotalIntervalTime();
		}

		protected virtual void RemovingEngine(IEngine engine)
		{
			TotalIntervalTime = 0;
		}

		#endregion

		#region Updates

		public void Update(float deltaTime)
		{
			if(IsSleeping)
				return;
			if(Engine?.CurrentSystem != this)
				return;
			if(updateLock)
				throw new InvalidOperationException($"{GetType().Name}.{nameof(Update)} cannot be called while already updating.");

			if(deltaIntervalTime > 0)
			{
				if(Engine.TotalVariableTime - totalIntervalTime < deltaIntervalTime)
					return;
				TotalIntervalTime += deltaIntervalTime;
				deltaTime = deltaIntervalTime;
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

		public TimeStep UpdateState
		{
			get => updateState;
			private set
			{
				if(updateState == value)
					return;
				var previous = updateState;
				updateState = value;
				Message<IUpdateStateMessage<ISystem>>(new UpdateStateMessage<ISystem>(value, previous));
			}
		}

		public TimeStep TimeStep
		{
			get => timeStep;
			protected set
			{
				if(timeStep == value)
					return;
				var previous = timeStep;
				timeStep = value;
				Message<IUpdateStateMessage<ISystem>>(new UpdateStateMessage<ISystem>(value, previous));
			}
		}

		#endregion

		#region Sleeping

		public int Sleeping
		{
			get => sleeping;
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
			get => sleeping > 0;
			set
			{
				if(value)
					++Sleeping;
				else
					--Sleeping;
			}
		}

		#endregion

		#region Interval Time

		public float DeltaIntervalTime
		{
			get => deltaIntervalTime;
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
			get => totalIntervalTime;
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

		#endregion

		#region Priority

		public int Priority
		{
			get => priority;
			set
			{
				if(priority == value)
					return;
				int previous = priority;
				priority = value;
				Message<IPriorityMessage<ISystem>>(new PriorityMessage<ISystem>(value, previous));
			}
		}

		#endregion
	}
}