using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.References;

namespace ExtendedXmlSerializer.ExtensionModel
{
	static class Defaults
	{
		public static TypeInfo Reference { get; } = typeof(ReferenceIdentity).GetTypeInfo();
	}
}