using System.Collections.ObjectModel;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	sealed class Pipeline<T> : Collection<IAlteration<T>>, IPipeline<T> {}
}