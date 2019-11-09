using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	interface ISingletonLocator : IParameterizedSource<TypeInfo, object> {}
}