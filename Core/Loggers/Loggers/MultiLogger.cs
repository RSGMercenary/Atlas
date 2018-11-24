using System;
using System.Collections.Generic;

namespace Atlas.Core.Loggers
{
	public class MultiLogger : ReadLogger
	{
		private readonly IEnumerable<ILogger> loggers;

		public MultiLogger(params ILogger[] loggers) : this(loggers as IEnumerable<ILogger>)
		{

		}

		public MultiLogger(IEnumerable<ILogger> loggers)
		{
			this.loggers = loggers;
		}

		public override void Info(object message, int skipFrames = 0)
		{
			++skipFrames;
			foreach(var logger in loggers)
				logger.Info(message, skipFrames);
		}

		public override void Warning(object message, int skipFrames = 0)
		{
			++skipFrames;
			foreach(var logger in loggers)
				logger.Warning(message, skipFrames);
		}

		public override void Error(object message, int skipFrames = 0)
		{
			++skipFrames;
			foreach(var logger in loggers)
				logger.Error(message, skipFrames);
		}

		public override void Exception(Exception exception, int skipFrames = 0)
		{
			++skipFrames;
			foreach(var logger in loggers)
				logger.Exception(exception, skipFrames);
		}
	}
}
