using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// A specialized selector that is intended to retrieve the root instance provided during serialization with the
	/// <see cref="ISerializer.Serialize" /> call.
	/// </summary>
	public interface IRootInstances : IParameterizedSource<object, object>, IAssignable<object, object> {}
}