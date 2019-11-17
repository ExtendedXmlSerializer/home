using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// Used by internal infrastructure to create a new content writer from a provided delegate.  Not intended to be used
	/// for external consumers.
	/// </summary>
	public interface IContentWriters : IParameterizedSource<Func<object, string>, IWriter> {}
}