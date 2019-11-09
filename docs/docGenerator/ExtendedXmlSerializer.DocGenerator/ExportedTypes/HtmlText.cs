namespace ExtendedXmlSerializer.DocGenerator.ExportedTypes
{
	using System.IO;
	using System.Linq;
	using System.Text;

	public class HtmlText : BaseExportedType
	{
		public override ExportedType ExportedType => ExportedType.Html;

		public override void AddHeader(string header)
		{
			Builder.AppendLine($"<h2>{header}</h2>");

			Builder.AppendLine();
		}

		public override void Add(string line)
		{
			Builder.AppendLine($"<p>{ReplaceCode(line)}</p>");
		}

		public override void AddList(params string[] list)
		{
			Builder.AppendLine();
			Builder.AppendLine("<ul>");
			foreach (var item in list)
			{
				Builder.AppendLine($"<li>{ReplaceCode(item)}</li>");
			}
			Builder.AppendLine("</ul>");
			Builder.AppendLine();
		}

		public override void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = ExtractFromString(fileContend, "// " + section, "// End" + section).SelectMany(p => p.Split('\n')).ToList();

			Builder.AppendLine();
			Builder.AppendLine(format == CodeFormat.Cs ? "<pre lang=\"cs\">" : "<pre lang=\"xml\">");

			var minIndent = code.Min(GetIndent);

			foreach (var item in code.Skip(code.First() == "\r" ? 1 : 0))
			{
				NormalizeIndent(System.Net.WebUtility.HtmlEncode(item), minIndent);
			}
			Builder.AppendLine("</pre>");
			Builder.AppendLine();
		}

		public override void AddCode(string file, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = fileContend.Split('\n').ToList();

			Builder.AppendLine();
			Builder.AppendLine(format == CodeFormat.Cs ? "<pre lang=\"cs\">" : "<pre lang=\"xml\">");

			var minIndent = code.Min(GetIndent);

			foreach (var item in code.Skip(code.First() == "\r" ? 1 : 0))
			{
				NormalizeIndent(System.Net.WebUtility.HtmlEncode(item), minIndent);
			}
			Builder.AppendLine("</pre>");
			Builder.AppendLine();
		}

		public override string ToString()
		{
			return Builder.ToString();
		}

		string ReplaceCode(string line)
		{
			StringBuilder result = new StringBuilder();

			bool first = true;
			foreach (var character in line)
			{
				if ('`'.Equals(character))
				{
					result.Append(first ? "<code>": "</code>");
					first = !first;
				}
				else
				{
					result.Append(character);
				}
			}

			return result.ToString();
		}

	}
}