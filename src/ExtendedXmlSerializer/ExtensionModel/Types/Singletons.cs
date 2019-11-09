using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class Singletons : ReferenceCache<PropertyInfo, object>, ISingletons
	{
		public static Singletons Default { get; } = new Singletons();

		Singletons() : base(x => x.GetValue(null)) {}
	}
}