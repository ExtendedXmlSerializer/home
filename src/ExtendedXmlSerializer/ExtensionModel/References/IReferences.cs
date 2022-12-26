using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	/// <summary>
	/// Specialized selector used to access references.
	/// </summary>
	[Obsolete("This interface has been deprecated and will be removed in a future version")]
	public interface IReferences : IParameterizedSource<object, ImmutableArray<object>> {}
}