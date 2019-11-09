using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class TypeConfigurationContext : DelegatedSource<TypeInfo>, ITypeConfigurationContext
	{
		public TypeConfigurationContext(IRootContext root, TypeInfo type) : base(type.Self)
		{
			Root = root;
		}

		public IRootContext Root { get; }
		public IContext Parent => Root;

		public IExtendedXmlSerializer Create() => Root.Create();
	}
}