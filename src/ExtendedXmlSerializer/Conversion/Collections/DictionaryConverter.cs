using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class DictionaryConverter : EnumerableConverter<IDictionary>
	{
		public DictionaryConverter(IConverter item, IActivator activator, TypeInfo classification)
			: base(item, activator, classification) {}

		protected override IEnumerator Get(IDictionary instance) => instance.GetEnumerator();
	}
}