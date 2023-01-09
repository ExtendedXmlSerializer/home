using ExtendedXmlSerializer.Core;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class StructureConverter<T> : IConverter<T?> where T : struct
	{
		readonly IConverter<T> _converter;

		public StructureConverter(IConverter<T> converter) => _converter = converter;

		public bool IsSatisfiedBy(TypeInfo parameter) => _converter.IsSatisfiedBy(parameter);

		public T? Parse(string data) => data.NullIfEmpty() != null ? _converter.Parse(data) : null;

		public string Format(T? instance) => instance.HasValue ? _converter.Format(instance.Value) : null;
	}
}