using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class RootContextExtension : ISerializerExtension
	{
		public RootContextExtension(IRootContext root) => Root = root;

		public IRootContext Root { get; }

		public IServiceRepository Get(IServiceRepository parameter) => parameter;

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}