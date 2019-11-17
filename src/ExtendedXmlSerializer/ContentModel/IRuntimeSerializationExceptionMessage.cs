using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel
{
	/// <summary>
	/// Used during runtime serialization to retrieve the message displayed when an exception is encountered during deserialization.
	/// </summary>
	public interface IRuntimeSerializationExceptionMessage : IParameterizedSource<TypeInfo, string> {}
}