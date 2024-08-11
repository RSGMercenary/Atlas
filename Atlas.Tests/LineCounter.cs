namespace Atlas.Tests;

public static class LineCounter
{
	public static void FileLines()
	{
		using var dialog = new OpenFileDialog();
		dialog.InitialDirectory = GetStartingPath();
		if(dialog.ShowDialog() != DialogResult.OK)
			return;
		WriteLine(dialog.FileName.Split('\\').Last(), LineCount(dialog.FileName));
	}

	public static void FolderLines(int titlePad = 70, int linePad = 4, bool sort = false)
	{
		using var dialog = new FolderBrowserDialog();
		dialog.SelectedPath = GetStartingPath();
		if(dialog.ShowDialog() != DialogResult.OK)
			return;

		var folderPath = dialog.SelectedPath;
		var totalLines = 0;

		var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories)
			.Select(file => new KeyValuePair<string, int>(file, LineCount(file)));
		if(sort)
			files = files.OrderBy(pair => pair.Value);

		foreach(var file in files)
		{
			totalLines += file.Value;
			if(file.Value > 0)
				WriteLine(file.Key.Replace($@"{folderPath}\", ""), file.Value, titlePad, linePad);
		}
		Console.WriteLine("".PadRight(titlePad + linePad + 3, '-'));
		WriteLine("Total Lines Of Code", totalLines, titlePad, linePad);
	}

	private static int LineCount(string file)
	{
		return File.ReadLines(file).Count();
	}

	private static string GetStartingPath()
	{
		return Directory.CreateDirectory(Environment.CurrentDirectory).Parent.Parent.FullName;
	}

	private static void WriteLine(string title, int count, int titlePad = 0, int linePad = 0)
	{
		Console.WriteLine($"{title.PadRight(titlePad)} = {count.ToString().PadLeft(linePad)}");
	}
}