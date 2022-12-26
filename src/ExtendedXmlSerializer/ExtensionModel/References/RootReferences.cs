using ExtendedXmlSerializer.ContentModel.Format;
using System;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class RootReferences : IRootReferences
	{
		readonly IReferenceView _references;
		readonly IRootInstances _root;

		public RootReferences(IReferenceView references, IRootInstances root)
		{
			_references = references;
			_root       = root;
		}

		public IReadOnlyCollection<object> Get(IFormatWriter parameter)
		{
			var root = _root.Get(parameter.Get());
			var result = root != null
				             ? _references.Get(root).Encountered
				             : (IReadOnlyCollection<object>)Array.Empty<object>();
			return result;
		}
	}
}