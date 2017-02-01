using System.Collections;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class DictionaryConverter : EnumerableConverter<IDictionary>
	{
		public DictionaryConverter(IConverter item, IActivator activator) : base(item, activator) {}

		protected override IEnumerator Get(IDictionary instance) => instance.GetEnumerator();
	}
}