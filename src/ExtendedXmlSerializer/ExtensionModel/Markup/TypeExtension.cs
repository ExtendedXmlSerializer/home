using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	/// <summary>
	/// Markup Extension that resolves to a type.
	/// </summary>
	[UsedImplicitly]
	public sealed class TypeExtension : IMarkupExtension
	{
		readonly string _typeName;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="typeName">The qualified type name.</param>
		public TypeExtension(string typeName) => _typeName = typeName;

		/// <inheritdoc />
		public object ProvideValue(System.IServiceProvider serviceProvider)
			=> serviceProvider.GetValid<IReflectionParser>()
			                  .Get(_typeName)
			                  .AsValid<TypeInfo>();
	}
}