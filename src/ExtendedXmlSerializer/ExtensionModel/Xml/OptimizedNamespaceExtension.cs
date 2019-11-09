using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class OptimizedNamespaceExtension : ISerializerExtension
	{
		public static OptimizedNamespaceExtension Default { get; } = new OptimizedNamespaceExtension();

		OptimizedNamespaceExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IObjectIdentifiers, ObjectIdentifiers>()
			            .Decorate<IFormatWriters<System.Xml.XmlWriter>, OptimizedWriters>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}