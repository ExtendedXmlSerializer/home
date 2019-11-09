using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	interface IDeferredCommand : ICommand<object>, ISource<object> {}
}