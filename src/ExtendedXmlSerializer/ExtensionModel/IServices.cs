using System;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// Represents a services container for the extension model.  This is responsible for querying and registering
	/// services, as well as finally disposing and cleaning all resources once the serializer has been constructed.  This
	/// is passed in as the parameter to post-registration during the extension processing.
	/// </summary>
	public interface IServices : IServiceRepository, IServiceProvider, IDisposable {}
}