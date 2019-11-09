using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	public interface IElement : IParameterizedSource<TypeInfo, IWriter> {}

	/*public interface IElements<in T> : ISource<IWriter<T>> {}*/
}