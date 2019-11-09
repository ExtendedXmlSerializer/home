using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	[UsedImplicitly]
	public sealed class StaticExtension : IMarkupExtension
	{
		readonly ISingletons _singletons;
		readonly string      _memberPath;

		public StaticExtension(string typeName, string memberName)
			: this(string.Concat(typeName, DefaultClrDelimiters.Default.Separator, memberName)) {}

		public StaticExtension(string memberPath) : this(Singletons.Default, memberPath) {}

		StaticExtension(ISingletons singletons, string memberPath)
		{
			_singletons = singletons;
			_memberPath = memberPath;
		}

		public object ProvideValue(System.IServiceProvider serviceProvider)
			=> _singletons.Get(serviceProvider.GetValid<IReflectionParser>()
			                                  .Get(_memberPath)
			                                  .AsValid<PropertyInfo>());
	}
}