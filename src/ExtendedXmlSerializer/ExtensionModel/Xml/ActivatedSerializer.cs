using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ActivatedSerializer : Activated<ISerializer>, ISerializer
	{
		public ActivatedSerializer(Type objectType, TypeInfo targetType) : base(objectType, targetType,
		                                                                        typeof(GenericSerializerAdapter<>)) {}

		static object Throw() =>
			throw new
				NotSupportedException("This serializer is used as a marker to activate another serializer at runtime.");

		public object Get(IFormatReader parameter) => Throw();

		public void Write(IFormatWriter writer, object instance)
		{
			Throw();
		}
	}
}