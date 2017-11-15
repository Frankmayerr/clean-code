using System;
using System.CodeDom;
using System.IO;
using System.Runtime.InteropServices;

namespace Markdown
{
	class Program
	{
		static void Main(string[] args)
		{
			var fileinput = "";
			var fileoutput = "";
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-f" && i + 1 < args.Length)
					fileinput = args[i + 1];
				if (args[i] == "-html" && i + 1 < args.Length)
					fileoutput = args[i + 1];
			}

			var md = new Md();
			try
			{
				var line = "";
				using (var sr = new StreamReader(fileinput))
					using (var sw = new StreamWriter(fileoutput))
						while ((line = sr.ReadLine()) != null)
							sw.WriteLine(md.RenderToHtml(line));
			}
			catch (Exception e)
			{
				Console.WriteLine("Wrong Files (or not found)");
			}
		}
	}
}