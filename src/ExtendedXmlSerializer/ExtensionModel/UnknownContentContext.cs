using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;
using System;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class UnknownContentContext
	{
		readonly IConfigurationContainer _container;

		public UnknownContentContext(IConfigurationContainer container) => _container = container;

		public IConfigurationContainer Continue() => Call(ContinueOnUnknownContent.Default.Execute);

		public IConfigurationContainer Throw() => Call(ThrowOnUnknownContent.Default.Execute);

		public IConfigurationContainer Call(Action<IFormatReader> callback)
			=> _container.Extend(new UnknownContentHandlingExtension(callback));
	}
}