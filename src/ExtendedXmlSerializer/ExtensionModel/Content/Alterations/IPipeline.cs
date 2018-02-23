using System.Collections.Generic;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	public interface IPipeline<T> : ICollection<IAlteration<T>> {}
}