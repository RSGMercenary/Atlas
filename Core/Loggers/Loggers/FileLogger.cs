using System.IO;

namespace Atlas.Core.Loggers
{
	public class FileLogger : WriteLogger
	{
		public string Path { get; set; }

		public FileLogger() : this(false) { }
		public FileLogger(bool verbose) : this(verbose, "log.txt") { }
		public FileLogger(string path) : this(false, path) { }
		public FileLogger(bool verbose, string path) : base(verbose)
		{
			Path = path;
			//To-DO Possibly limit the file size or number of lines.
			File.Delete(Path);
		}

		protected override void Log(object message)
		{
			using(var writer = File.AppendText(Path))
				writer.WriteLine(message);
		}
	}
}