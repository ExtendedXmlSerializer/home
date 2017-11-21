namespace ExtendedXmlSerializer.DocGenerator.ExportedTypes
{
	using System.Collections.Generic;
	using System.Linq;

	public class ExportedTypeManager : IExportedType
	{
		List<IExportedType> ExportedTypeList = new List<IExportedType>();

		public ExportedTypeManager(List<IExportedType> exportedTypeList)
		{
			this.ExportedTypeList = exportedTypeList;
		}

		public ExportedType ExportedType => ExportedType.Manager;

		public void AddHeader(string header)
		{
			ExportedTypeList.ForEach(p=>p.AddHeader(header));
		}

		public void Add(string line)
		{
			ExportedTypeList.ForEach(p => p.Add(line));
		}

		public void AddList(params string[] list)
		{
			ExportedTypeList.ForEach(p => p.AddList(list));
		}

		public void AddCode(string file, string section, CodeFormat format = CodeFormat.Cs)
		{
			ExportedTypeList.ForEach(p => p.AddCode(file, section, format));
		}

		public void AddCode(string file, CodeFormat format = CodeFormat.Cs)
		{
			ExportedTypeList.ForEach(p => p.AddCode(file, format));
		}

		public void Save(ExportedType type, string path)
		{
			ExportedTypeList.FirstOrDefault(p => p.ExportedType == type)
				.Save(type, path);
		}

		public string ToString(ExportedType type)
		{
			return ExportedTypeList.FirstOrDefault(p => p.ExportedType == type)
				.ToString();
		}
	}
}
