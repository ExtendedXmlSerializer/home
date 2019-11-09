using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	public interface ISerializers : IParameterizedSource<TypeInfo, ISerializer> {}
}