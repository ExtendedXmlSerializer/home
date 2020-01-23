using ExtendedXmlSerializer.ContentModel.Format;
using System.Collections.Immutable;

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

		public ImmutableArray<object> Get(IFormatWriter parameter)
		{
			var root   = _root.Get(parameter.Get());
			var result = root != null ? _references.Get(root) : ImmutableArray<object>.Empty;
			return result;
		}
	}
}