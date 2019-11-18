using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Represents a store keyed on type metadata.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	public interface ITypedTable<T> : IMetadataTable<TypeInfo, T> {}
}