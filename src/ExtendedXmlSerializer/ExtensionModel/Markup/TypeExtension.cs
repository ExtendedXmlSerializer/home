using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	[UsedImplicitly]
	public sealed class TypeExtension : IMarkupExtension
	{
		readonly string _typeName;

		public TypeExtension(string typeName)
		{
			_typeName = typeName;
		}

		public object ProvideValue(System.IServiceProvider serviceProvider)
			=> serviceProvider.GetValid<IReflectionParser>()
			                  .Get(_typeName)
			                  .AsValid<TypeInfo>();
	}
}