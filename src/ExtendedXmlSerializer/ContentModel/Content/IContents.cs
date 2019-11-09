using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	public interface IContents : IParameterizedSource<TypeInfo, ISerializer> {}

	public interface IContents<T> : ISource<ISerializer<T>> {}
}