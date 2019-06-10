using System.Diagnostics;

namespace Atlas.Core.Loggers
{
	public class DebugLogger : WriteLogger
	{
		public DebugLogger() { }
		public DebugLogger(bool verbose) : base(verbose) { }

		protected override void Log(object message)
		{
			Debug.WriteLine(message);
		}
	}
}