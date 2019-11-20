namespace ExtendedXmlSerializer.DocGenerator.ExportedTypes
{
	public enum ExportedType
	{
		ReStructuredText,
		Html,
		Manager
	}

	public interface IExportedType
	{
		ExportedType ExportedType { get; }
		void AddHeader(string header);

		void Add(string line);

		void AddList(params string[] list);

		void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs);

		void AddCode(string file, CodeFormat format = CodeFormat.Cs);

		void Save(ExportedType type, string path);
	}
}