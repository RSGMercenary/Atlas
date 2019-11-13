using System;

namespace Atlas.Core.Loggers
{
	public class ConsoleLogger : WriteLogger
	{
		public ConsoleLogger() : this(true) { }
		public ConsoleLogger(bool verbose) : base(verbose) { }

		protected override void Log(object message)
		{
			Console.WriteLine(message);
		}
	}
}