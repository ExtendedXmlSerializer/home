using ExtendedXmlSerializer.ExtensionModel.References;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	static class Defaults
	{
		public static TypeInfo Reference { get; } = typeof(ReferenceIdentity).GetTypeInfo();
	}
}