using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Used to compare types.
	/// </summary>
	public interface ITypeComparer : IEqualityComparer<TypeInfo> {}
}