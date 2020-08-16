using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	interface IActivationContext : ITableSource<string, object>, IList, IActivator {}
}