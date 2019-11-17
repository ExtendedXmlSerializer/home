using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// The heart and soul of the extension model.  This performs any registrations during serializer activation and once
	/// the serializer is created, performs any necessary post-processing with its <see cref="ICommand{T}"/>
	/// implementation.
	/// </summary>
	public interface ISerializerExtension : IAlteration<IServiceRepository>, ICommand<IServices> {}
}