

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
			this._builder.AppendLine(header);
			for (int i = 0; i < header.Length; i++)
			{
				this._builder.Append("=");
			}
			this._builder.AppendLine();
			this._builder.AppendLine();
		}

		public override void Add(string line)
		{
			this._builder.AppendLine(line);
		}

		public override void AddList(params string[] list)
		{
			this._builder.AppendLine();
			foreach (var item in list)
			{
				this._builder.AppendLine("* "+item);
			}
			this._builder.AppendLine();
		}

		public override void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = ExtractFromString(fileContend, "// " + section, "// End" + section).SelectMany(p=>p.Split('\n')).ToList();

			this._builder.AppendLine();
			this._builder.AppendLine(format == CodeFormat.Cs ? ".. sourcecode:: csharp" : ".. sourcecode:: xml");
			this._builder.AppendLine();

			var minIndent = code.Min(GetIndent);

			foreach (var item in code.Skip(code.First() == "\r" ? 1 : 0))
			{
				this.NormalizeIndent(item, minIndent);
			}
			this._builder.AppendLine();
		}

		public override void AddCode(string file, CodeFormat format = CodeFormat.Cs)
		{
			string fileContend = File.ReadAllText(file);
			var code = fileContend.Split('\n').ToList();

			this._builder.AppendLine();
			this._builder.AppendLine(format == CodeFormat.Cs ? ".. sourcecode:: csharp" : ".. sourcecode:: xml");
			this._builder.AppendLine();

			var minIndent = code.Min(GetIndent);

			foreach (var item in code.Skip(code.First() == "\r" ? 1 : 0))
			{
				this.NormalizeIndent(item, minIndent);
			}
			this._builder.AppendLine();
		}

		public override string ToString()
		{
			return this._builder.ToString();
		}
	}
}
