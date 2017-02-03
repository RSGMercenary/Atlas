using System;

namespace Atlas.Testing
{
	static class AtlasLogger
	{
		private static AtlasTraceListener trace = new AtlasTraceListener("AtlasLog.log");

		public static void Error(string message, string module)
		{
			Log(message, "ERROR", module);
		}

		public static void Error(Exception error, string module)
		{
			Log(error.Message + Environment.NewLine + error.StackTrace, "ERROR", module);
		}

		public static void Warning(string message, string module)
		{
			Log(message, "WARNING", module);
		}

		public static void Info(string message, string module)
		{
			Log(message, "INFO", module);
		}

		public static void Log(string message, string type, string module)
		{
			trace.WriteLine(string.Format("[{0}] [{1}] [{2}]\n{3}\n",
								  DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"),
								  type,
								  module,
								  message));
		}
	}
}
