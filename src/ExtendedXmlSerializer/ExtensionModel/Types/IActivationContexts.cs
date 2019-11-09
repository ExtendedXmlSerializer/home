using System.Collections.Generic;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	interface IActivationContexts : IParameterizedSource<IDictionary<string, object>, IActivationContext> {}
}