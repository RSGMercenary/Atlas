using System;

namespace Atlas.Core.Loggers
{
	public static class Log
	{
		private static ILogger logger = new DebugLogger();

		/// <summary>
		/// This Logger will be used everywhere. Set this
		/// to any ILogger you want to log messages with.
		/// </summary>
		public static ILogger Logger
		{
			get { return logger; }
			set { logger = value ?? new DebugLogger(); }

		}

		public static void Info(object info)
		{
			Logger.Info(info, 1);
		}

		public static void Warning(object warning)
		{
			Logger.Warning(warning, 1);
		}

		public static void Error(object error)
		{
			Logger.Error(error, 1);
		}

		public static void Exception(Exception exception)
		{
			Logger.Exception(exception, 1);
		}
	}
}
