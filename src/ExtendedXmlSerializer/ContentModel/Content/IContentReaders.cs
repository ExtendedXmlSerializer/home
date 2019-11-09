using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	public interface IContentReaders : IParameterizedSource<Func<string, object>, IReader> {}
}