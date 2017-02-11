using System.Reflection;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel.Converters
{
	public interface IConverter<T> : ISpecification<TypeInfo>
	{
		T Load(string data);

		string Save(T instance);
	}

	public interface IConverter : ISpecification<TypeInfo>
	{
		object Load(string data);

		string Save(object instance);
	}
}