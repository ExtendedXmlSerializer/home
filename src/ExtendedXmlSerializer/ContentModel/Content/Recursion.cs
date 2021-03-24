using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Recursion : IContents
	{
		readonly IContents      _contents;
		readonly ISet<TypeInfo> _types;

		public Recursion(IContents contents) : this(contents, new HashSet<TypeInfo>()) {}

		public Recursion(IContents contents, ISet<TypeInfo> types)
		{
			_contents = contents;
			_types    = types;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var result = _types.Add(parameter)
				             ? _contents.Get(parameter)
				             : new DeferredSerializer(_contents.Build(parameter));
			_types.Remove(parameter);
			return result;
		}
	}
}