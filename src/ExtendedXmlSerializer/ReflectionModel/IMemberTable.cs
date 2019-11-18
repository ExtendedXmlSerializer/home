using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Represents a store that is keyed on member metadata.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	public interface IMemberTable<T> : IMetadataTable<MemberInfo, T> {}
}