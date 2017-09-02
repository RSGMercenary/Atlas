using Atlas.Engine.Signals;

namespace Atlas.Testing.CAT
{
	class Condition : Action, ICondition
	{
		private bool not = false;
		private bool isTrue = false;
		private Signal<ICondition, bool> isTrueChanged = new Signal<ICondition, bool>();

		private bool immediate = true;
		private bool listen = true;

		public bool Not
		{
			get { return not; }
			set { not = value; }
		}

		protected virtual void Check()
		{

		}

		public bool IsTrue
		{
			get { return isTrue != not; }
			protected set
			{
				if(isTrue == value)
					return;
				isTrue = value;
				isTrueChanged.Dispatch(this, value);
			}
		}

		public ISignal<ICondition, bool> IsTrueChanged
		{
			get { return isTrueChanged; }
		}

		public bool Status
		{
			get { return isTrue != not; }
		}
	}
}
