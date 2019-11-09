using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class RootReferences : IRootReferences
	{
		readonly IReferences    _references;
		readonly IRootInstances _root;

		public RootReferences(IReferences references, IRootInstances root)
		{
			_references = references;
			_root       = root;
		}

		public ImmutableArray<object> Get(IFormatWriter parameter) => _references.Get(_root.Get(parameter.Get()));
	}
}