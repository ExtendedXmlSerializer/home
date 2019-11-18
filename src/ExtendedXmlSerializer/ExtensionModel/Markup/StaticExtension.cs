using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Types;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	/// <summary>
	/// Markup extension that resolves a static member type from a namespace and member expression.
	/// </summary>
	[UsedImplicitly]
	public sealed class StaticExtension : IMarkupExtension
	{
		readonly ISingletons _singletons;
		readonly string      _memberPath;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="typeName">The expression that resolves to the type that holds the static member.</param>
		/// <param name="memberName">The name of the public static member.</param>
		public StaticExtension(string typeName, string memberName)
			: this(string.Concat(typeName, DefaultClrDelimiters.Default.Separator, memberName)) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="memberPath">The qualified path to the public static member.</param>
		public StaticExtension(string memberPath) : this(Singletons.Default, memberPath) {}

		StaticExtension(ISingletons singletons, string memberPath)
		{
			_singletons = singletons;
			_memberPath = memberPath;
		}

		/// <inheritdoc />
		public object ProvideValue(System.IServiceProvider serviceProvider)
			=> _singletons.Get(serviceProvider.GetValid<IReflectionParser>()
			                                  .Get(_memberPath)
			                                  .AsValid<PropertyInfo>());
	}
}