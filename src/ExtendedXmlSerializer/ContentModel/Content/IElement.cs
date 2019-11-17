using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// Root-level component that, when provided a type, will select a writer used to emit an element tag along with any
	/// necessary namespace information or other attributes necessary to support it.
	/// </summary>
	public interface IElement : IParameterizedSource<TypeInfo, IWriter> {}
}