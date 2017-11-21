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
			this._builder.AppendLine($"<h2>{header}</h2>");

			this._builder.AppendLine();
		}

		public override void Add(string line)
		{
			this._builder.AppendLine($"<p>{this.ReplaceCode(line)}</p>");
		}

		public override void AddList(params string[] list)
		{
			this._builder.AppendLine();
			this._builder.AppendLine("<ul>");
			foreach (var item in list)
			{
				this._builder.AppendLine($"<li>{this.ReplaceCode(item)}</li>");
			}
			this._builder.AppendLine("</ul>");
			this._builder.AppendLine();
		}

		public override void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = ExtractFromString(fileContend, "// " + section, "// End" + section).SelectMany(p => p.Split('\n')).ToList();

			this._builder.AppendLine();
			this._builder.AppendLine(format == CodeFormat.Cs ? "<pre lang=\"cs\">" : "<pre lang=\"xml\">");

			var minIndent = code.Min(GetIndent);

			foreach (var item in code.Skip(code.First() == "\r" ? 1 : 0))
			{
				this.NormalizeIndent(System.Net.WebUtility.HtmlEncode(item), minIndent);
			}
			this._builder.AppendLine("</pre>");
			this._builder.AppendLine();
		}

		public override void AddCode(string file, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = fileContend.Split('\n').ToList();

			this._builder.AppendLine();
			this._builder.AppendLine(format == CodeFormat.Cs ? "<pre lang=\"cs\">" : "<pre lang=\"xml\">");

			var minIndent = code.Min(GetIndent);

			foreach (var item in code.Skip(code.First() == "\r" ? 1 : 0))
			{
				this.NormalizeIndent(System.Net.WebUtility.HtmlEncode(item), minIndent);
			}
			this._builder.AppendLine("</pre>");
			this._builder.AppendLine();
		}

		public override string ToString()
		{
			return this._builder.ToString();
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