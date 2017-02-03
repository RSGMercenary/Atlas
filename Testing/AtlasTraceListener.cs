using System.Diagnostics;
using System.IO;

namespace Atlas.Testing
{
	class AtlasTraceListener:TextWriterTraceListener
	{
		private string filePath = "";
		private bool append = false;
		private StreamWriter writer;

		public AtlasTraceListener(string filePath = "", bool append = false)
		{
			FilePath = filePath;
			Append = append;
		}

		private void ChangeWriter()
		{
			if(writer != null)
			{
				writer.Dispose();
				writer = null;
			}
			if(!string.IsNullOrWhiteSpace(filePath))
			{
				writer = new StreamWriter(filePath, append);
				writer.AutoFlush = true;
			}
		}

		public string FilePath
		{
			get
			{
				return filePath;
			}
			set
			{
				if(filePath != value)
				{
					filePath = value;
					ChangeWriter();
				}
			}
		}

		public bool Append
		{
			get
			{
				return append;
			}
			set
			{
				if(append != value)
				{
					append = value;
					ChangeWriter();
				}
			}
		}

		public override void Write(string message)
		{
			if(writer != null)
				writer.Write(message);
		}

		public override void WriteLine(string message)
		{
			if(writer != null)
				writer.WriteLine(message);
		}

		public override void Close()
		{
			if(writer != null)
				writer.Close();
		}
	}
}
