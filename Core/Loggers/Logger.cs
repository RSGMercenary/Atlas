using System;
using System.Diagnostics;

namespace Atlas.Core.Loggers
{
	public abstract class ReadLogger : ILogger
	{
		public abstract void Info(object message, int skipFrames = 0);
		public abstract void Warning(object message, int skipFrames = 0);
		public abstract void Error(object message, int skipFrames = 0);
		public abstract void Exception(Exception exception, int skipFrames = 0);
	}

	public abstract class WriteLogger : ReadLogger
	{
		public bool Verbose { get; set; } = true;

		protected WriteLogger(bool verbose) { Verbose = verbose; }

		public override void Info(object message, int skipFrames = 0)
		{
			Log(message, LogLevel.Info, ++skipFrames);
		}

		public override void Warning(object message, int skipFrames = 0)
		{
			Log(message, LogLevel.Warning, ++skipFrames);
		}

		public override void Error(object message, int skipFrames = 0)
		{
			Log(message, LogLevel.Error, ++skipFrames);
		}

		public override void Exception(Exception exception, int skipFrames = 0)
		{
			Log(exception.Message + Environment.NewLine + exception.StackTrace, LogLevel.Exception, ++skipFrames);
		}

		//[MethodImpl(MethodImplOptions.NoInlining)]
		private void Log(object message, LogLevel level, int skipFrames)
		{
			if(Verbose)
			{
				var logLevel = level.ToString().ToUpper();
				var timestamp = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss.ffff");
				var method = new StackTrace(++skipFrames).GetFrame(0).GetMethod();
				var location = $"{method.DeclaringType.FullName} {method.Name}()";
				var newLine = Environment.NewLine;
				message = $"[{logLevel}] [{timestamp}] [{location}]{newLine}{message}{newLine}";
			}
			Log(message);
		}

		protected abstract void Log(object message);
	}
}