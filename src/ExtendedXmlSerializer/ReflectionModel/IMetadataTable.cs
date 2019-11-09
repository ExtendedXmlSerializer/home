using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public interface IMetadataTable<in TMetadata, TValue> : ITableSource<TMetadata, TValue>
		where TMetadata : MemberInfo {}
}