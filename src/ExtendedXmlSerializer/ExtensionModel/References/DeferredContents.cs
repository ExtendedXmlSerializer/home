using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredContents : IContents
	{
		readonly Func<IContents> _contents;

		public DeferredContents(Func<IContents> contents) => _contents = contents;

		public ISerializer Get(TypeInfo parameter) => _contents()
			.Get(parameter);
	}
}