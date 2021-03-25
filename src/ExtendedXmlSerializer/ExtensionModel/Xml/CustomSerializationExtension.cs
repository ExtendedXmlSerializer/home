using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class CustomSerializationExtension : ISerializerExtension
	{
		public CustomSerializationExtension() : this(new CustomXmlSerializers(), new CustomSerializers(),
		                                             new MemberCustomSerializers()) {}

		public CustomSerializationExtension(ICustomXmlSerializers xmlSerializers, ICustomSerializers types,
		                                    ICustomMemberSerializers members)
		{
			XmlSerializers = xmlSerializers;
			Types          = types;
			Members        = members;
		}

		public ICustomXmlSerializers XmlSerializers { get; }

		public ICustomSerializers Types { get; }

		public ICustomMemberSerializers Members { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> Extensions().Aggregate(parameter, (repository, serializer) => serializer.Get(repository))
			               .RegisterInstance(XmlSerializers)
			               .RegisterInstance(Types)
			               .RegisterInstance(Members)
			               .Register<IContainsCustomSerialization, ContainsCustomSerialization>()
			               .Decorate<IContents, Contents>();

		void ICommand<IServices>.Execute(IServices parameter)
		{
			foreach (var extension in Extensions())
			{
				extension.Execute(parameter);
			}
		}

		IEnumerable<ISerializerExtension> Extensions() => new ISerializerExtension[] {XmlSerializers, Types, Members};

		sealed class Contents : IContents
		{
			readonly ICustomXmlSerializers _custom;
			readonly IContents             _contents;

			public Contents(ICustomXmlSerializers custom, IContents contents)
			{
				_custom   = custom;
				_contents = contents;
			}

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var custom = _custom.Get(parameter);
				var result = custom != null ? new Serializer(custom) : _contents.Get(parameter);
				return result;
			}

			sealed class Serializer : ContentModel.ISerializer
			{
				readonly IExtendedXmlCustomSerializer _custom;

				public Serializer(IExtendedXmlCustomSerializer custom) => _custom = custom;

				public object Get(IFormatReader parameter)
				{
					var reader = parameter.Get()
					                      .AsValid<System.Xml.XmlReader>();
					var subtree = reader.ReadSubtree();
					var element = XElement.Load(subtree);
					var result  = _custom.Deserialize(element);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
					=> _custom.Serializer(writer.Get()
					                            .AsValid<System.Xml.XmlWriter>(), instance);
			}
		}
	}
}