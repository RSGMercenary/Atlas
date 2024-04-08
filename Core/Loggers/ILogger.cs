using System;

namespace Atlas.Core.Loggers;

public interface ILogger
{
	/// <summary>
	/// Logs an INFO message.
	/// </summary>
	/// <param name="message"></param>
	void Info(object message, int skipFrames = 0);

	/// <summary>
	/// Logs a WARNING message.
	/// </summary>
	/// <param name="message"></param>
	void Warning(object message, int skipFrames = 0);

	/// <summary>
	/// Logs an ERROR message.
	/// </summary>
	/// <param name="message"></param>
	void Error(object message, int skipFrames = 0);

	/// <summary>
	/// Logs an ERROR exception.
	/// </summary>
	/// <param name="exception"></param>
	void Exception(Exception exception, int skipFrames = 0);
}