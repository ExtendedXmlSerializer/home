using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public interface ITypedTable<T> : IMetadataTable<TypeInfo, T> {}
}