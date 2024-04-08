using System;
using System.Collections.Generic;

namespace Atlas.Core.Loggers;

public static class Log
{
	private static List<ILogger> loggers = new List<ILogger> { new DebugLogger() };

	public static IEnumerable<ILogger> Loggers
	{
		get { return loggers; }
		set
		{
			loggers.Clear();
			loggers.AddRange(value);
		}
	}

	public static void Info(object info)
	{
		foreach(var logger in loggers)
			logger.Info(info, 1);
	}

	public static void Warning(object warning)
	{
		foreach(var logger in loggers)
			logger.Warning(warning, 1);
	}

	public static void Error(object error)
	{
		foreach(var logger in loggers)
			logger.Error(error, 1);
	}

	public static void Exception(Exception exception)
	{
		foreach(var logger in loggers)
			logger.Exception(exception, 1);
	}
}