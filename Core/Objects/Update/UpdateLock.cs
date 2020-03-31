using System;

namespace Atlas.Core.Objects.Update
{
	public class UpdateLock
	{
		private bool locked = false;

		public void Lock()
		{
			if(locked)
				throw new InvalidOperationException("Update is already locked, and can't be locked again.");
			locked = true;
		}

		public void Unlock() => locked = false;
	}
}