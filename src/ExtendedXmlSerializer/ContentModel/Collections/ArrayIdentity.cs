using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ArrayIdentity : IWriter<Array>
	{
		readonly IWriter<Array>      _identity;
		readonly IProperty<TypeInfo> _property;
		readonly TypeInfo            _element;

		public ArrayIdentity(IWriter<Array> identity, TypeInfo element) : this(identity, ItemTypeProperty.Default,
		                                                                       element) {}

		public ArrayIdentity(IWriter<Array> identity, IProperty<TypeInfo> property, TypeInfo element)
		{
			_identity = identity;
			_property = property;
			_element  = element;
		}

		public void Write(IFormatWriter writer, Array instance)
		{
			_identity.Write(writer, instance);
			_property.Write(writer, _element);
		}
	}
}