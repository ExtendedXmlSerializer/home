using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// A table that is keyed on reflection metadata.
	/// </summary>
	/// <typeparam name="TMetadata">The metadata type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	public interface IMetadataTable<in TMetadata, TValue> : ITableSource<TMetadata, TValue>
		where TMetadata : MemberInfo {}
}