using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	public interface IContentWriters : IParameterizedSource<Func<object, string>, IWriter> {}
}