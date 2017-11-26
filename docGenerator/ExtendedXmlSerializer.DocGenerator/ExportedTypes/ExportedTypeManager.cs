namespace ExtendedXmlSerializer.DocGenerator.ExportedTypes
{
	using System.Collections.Generic;
	using System.Linq;

	public class ExportedTypeManager : IExportedType
	{
		readonly List<IExportedType> _exportedTypeList;

		public ExportedTypeManager(List<IExportedType> exportedTypeList) => _exportedTypeList = exportedTypeList;

		public ExportedType ExportedType => ExportedType.Manager;

		public void AddHeader(string header)
		{
			_exportedTypeList.ForEach(p=>p.AddHeader(header));
		}

		public void Add(string line)
		{
			_exportedTypeList.ForEach(p => p.Add(line));
		}

		public void AddList(params string[] list)
		{
			_exportedTypeList.ForEach(p => p.AddList(list));
		}

		public void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
		{
			_exportedTypeList.ForEach(p => p.AddCode(file, section, format));
		}

		public void AddCode(string file, CodeFormat format = CodeFormat.Cs)
		{
			_exportedTypeList.ForEach(p => p.AddCode(file, format));
		}

		public void Save(ExportedType type, string path)
		{
			_exportedTypeList.FirstOrDefault(p => p.ExportedType == type)
				.Save(type, path);
		}

		public string ToString(ExportedType type)
		{
			return _exportedTypeList.FirstOrDefault(p => p.ExportedType == type)
				.ToString();
		}
	}
}
