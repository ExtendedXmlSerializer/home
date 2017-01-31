using System.Reflection;
using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion.Collections
{
	class CollectionItemConverter : NamedConverter
	{
		readonly IConverters _converters;

		public CollectionItemConverter(IConverters converters, IName elementName)
			: this(converters, elementName, converters.Get(elementName.Classification)) {}

		public CollectionItemConverter(IConverters converters, IName elementName, IConverter body) : base(elementName, body)
		{
			_converters = converters;
		}

		public override void Write(IWriter writer, object instance)
		{
			var type = instance.GetType();
			var actual = type.GetTypeInfo();
			if (Equals(actual, Name.Classification))
			{
				base.Write(writer, instance);
			}
			else
			{
				_converters.Get(actual).Write(writer, instance);
			}
		}
	}
}