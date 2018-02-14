using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomXmlSerializerAdapter<T> : GenerializedContentSerializer<T>
	{
		public CustomXmlSerializerAdapter(IExtendedXmlCustomSerializer custom) : base(new CustomXmlSerializer<T>(custom)) {}
	}

	sealed class CustomXmlSerializer<T> : IContentSerializer<T>
	{
		readonly IExtendedXmlCustomSerializer _custom;

		public CustomXmlSerializer(IExtendedXmlCustomSerializer custom) => _custom = custom;

		public T Get(IFormatReader parameter)
		{
			var reader = parameter.Get()
			                      .To<System.Xml.XmlReader>();
			var subtree = reader.ReadSubtree();
			var element = XElement.Load(subtree);
			var result = _custom.Deserialize(element).To<T>();
			return result;
		}

		public void Execute(ContentModel.Writing<T> parameter)
		{
			_custom.Serializer(parameter.Writer.Get()
			                            .To<System.Xml.XmlWriter>(), parameter.Instance);
		}
	}

	sealed class GenericCustomXmlSerializer<T> : IContentSerializer<T>
	{
		readonly IExtendedXmlCustomSerializer<T> _custom;

		public GenericCustomXmlSerializer(IExtendedXmlCustomSerializer<T> custom) => _custom = custom;

		public T Get(IFormatReader parameter)
		{
			var reader = parameter.Get()
			                      .To<System.Xml.XmlReader>();
			var subtree = reader.ReadSubtree();
			var element = XElement.Load(subtree);
			var result = _custom.Deserialize(element);
			return result;
		}

		public void Execute(ContentModel.Writing<T> parameter)
		{
			_custom.Serializer(parameter.Writer.Get()
			                            .To<System.Xml.XmlWriter>(), parameter.Instance);
		}
	}

}
