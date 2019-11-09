

// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace ExtendedXmlSerializer.DocGenerator.ExportedTypes
{
	using System.IO;
	using System.Linq;

	public class ReStructuredText : BaseExportedType
	{
		public override ExportedType ExportedType => ExportedType.ReStructuredText;

		public override void AddHeader(string header)
		{
			Builder.AppendLine(header);
			for (int i = 0; i < header.Length; i++)
			{
				Builder.Append("=");
			}
			Builder.AppendLine();
			Builder.AppendLine();
		}

		public override void Add(string line)
		{
			Builder.AppendLine(line);
		}

		public override void AddList(params string[] list)
		{
			Builder.AppendLine();
			foreach (var item in list)
			{
				Builder.AppendLine("* "+item);
			}
			Builder.AppendLine();
		}

		public override void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = ExtractFromString(fileContend, "// " + section, "// End" + section).SelectMany(p=>p.Split('\n')).ToList();

			Builder.AppendLine();
			Builder.AppendLine(format == CodeFormat.Cs ? ".. sourcecode:: csharp" : ".. sourcecode:: xml");
			Builder.AppendLine();

			var minIndent = code.Min(GetIndent);

			var enumerable = code.SkipWhile(string.IsNullOrWhiteSpace);
			foreach (var item in enumerable)
			{
				NormalizeIndent(item, minIndent);
			}
			Builder.AppendLine();
		}

		public override void AddCode(string file, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = fileContend.Split('\n').ToList();

			Builder.AppendLine();
			Builder.AppendLine(format == CodeFormat.Cs ? ".. sourcecode:: csharp" : ".. sourcecode:: xml");
			Builder.AppendLine();

			var minIndent = code.Min(GetIndent);

			var enumerable = code.SkipWhile(string.IsNullOrWhiteSpace);
			foreach (var item in enumerable)
			{
				NormalizeIndent(item, minIndent);
			}
			Builder.AppendLine();
		}

		public override string ToString()
		{
			return Builder.ToString();
		}
	}
}
