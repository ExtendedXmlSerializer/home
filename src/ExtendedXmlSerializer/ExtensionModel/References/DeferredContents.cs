using ExtendedXmlSerializer.ContentModel.Content;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredContents : IContents
	{
		readonly Func<IContents> _contents;

		public DeferredContents(Func<IContents> contents) => _contents = contents;

		public ContentModel.ISerializer Get(TypeInfo parameter) => _contents()
			.Get(parameter);
	}
}