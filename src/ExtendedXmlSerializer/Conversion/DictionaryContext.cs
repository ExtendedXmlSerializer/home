using System.Collections;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	class DictionaryContext : EnumerableContext<IDictionary>
	{
		public DictionaryContext(IElementContext item, IActivator activator, TypeInfo classification)
			: base(item, activator, classification) {}

		protected override IEnumerator Get(IDictionary instance) => instance.GetEnumerator();
	}
}