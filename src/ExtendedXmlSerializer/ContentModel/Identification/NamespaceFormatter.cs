using System.Reflection;
using System.Runtime.Serialization.Formatters;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class NamespaceFormatter : INamespaceFormatter
	{
		public static NamespaceFormatter Default { get; } = new NamespaceFormatter();

		NamespaceFormatter() : this(FormatterAssemblyStyle.Simple) {}

		[UsedImplicitly]
		public static NamespaceFormatter Full { get; } = new NamespaceFormatter(FormatterAssemblyStyle.Full);

		readonly FormatterAssemblyStyle _style;

		public NamespaceFormatter(FormatterAssemblyStyle style)
		{
			_style = style;
		}

		public string Get(TypeInfo type) => $"clr-namespace:{type.Namespace};assembly={Name(type)}";

		string Name(TypeInfo type)
		{
			var name = type.Assembly.GetName();
			switch (_style)
			{
				case FormatterAssemblyStyle.Simple:
					return name.Name;
				default:
					return name.ToString();
			}
		}
	}
}