using System.Reflection;
using ExtendedXmlSerialization.ConverterModel.Xml;

namespace ExtendedXmlSerialization.ConverterModel.Properties
{
	abstract class TypePropertyBase : FrameworkPropertyBase<TypeInfo>
	{
		protected TypePropertyBase(string displayName) : base(displayName) {}

		public override void Write(IXmlWriter writer, TypeInfo instance) => writer.Attribute(Name, writer.Get(instance));

		public override TypeInfo Get(IXmlReader reader)
		{
			var data = reader[Name];
			var name = reader.Get(data);
			var result = reader.Get(name);
			return result;
		}
	}
}