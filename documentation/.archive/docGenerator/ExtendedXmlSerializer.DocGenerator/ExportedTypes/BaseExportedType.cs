using System;

namespace ExtendedXmlSerializer.DocGenerator.ExportedTypes
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	public abstract class BaseExportedType : IExportedType
	{
		protected readonly StringBuilder Builder = new StringBuilder();

		public abstract ExportedType ExportedType { get; }

		public abstract void AddHeader(string header);

		public abstract void Add(string line);

		public abstract void AddList(params string[] list);

		public abstract void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs);

		public abstract void AddCode(string file, CodeFormat format = CodeFormat.Cs);

		public void Save(ExportedType type, string path)
		{
			var result = ToString();

			File.WriteAllText(path, result);
		}

		protected void NormalizeIndent(string line, int minIndent)
		{
			var currentIndent = GetIndent(line) - minIndent + 4;
			for (int i = 0; i < currentIndent; i++)
			{
				Builder.Append(' ');
			}
			Builder.AppendLine(line.Trim());
		}


		protected static int GetIndent(string line)
		{
			var result = 0;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '\t')
				{
					result += 4;
					continue;
				}
				if (line[i] == ' ')
				{
					result += 1;
					continue;
				}
				break;
			}
			return result;
		}

		protected static List<string> ExtractFromString(
			string text, string startString, string endString)
		{
			List<string> matched = new List<string>();
			int indexStart, indexEnd;
			bool exit = false;
			while (!exit)
			{
				indexStart = text.IndexOf(startString, StringComparison.Ordinal) + 1;
				indexEnd = text.IndexOf(endString, StringComparison.Ordinal) - 1;
				if (indexStart >= 0 && indexEnd >= 0)
				{
					matched.Add(text.Substring(indexStart + startString.Length,
						indexEnd - indexStart - startString.Length));
					text = text.Substring(indexEnd + endString.Length);
				}
				else
					exit = true;
			}
			return matched;
		}
	}
}