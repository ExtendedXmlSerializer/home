using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	/// <summary>
	/// Used to resolve a type from a provided identity.
	/// </summary>
	public interface ITypes : IParameterizedSource<IIdentity, TypeInfo> {}
}