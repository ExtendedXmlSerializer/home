using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using System.Reflection;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class ExistingInstanceExtension : ISerializerExtension
	{
		public static ExistingInstanceExtension Default { get; } = new ExistingInstanceExtension();

		ExistingInstanceExtension() {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IActivation, Activation>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Activation : IActivation
		{
			readonly IActivation _activation;

			public Activation(IActivation activation) => _activation = activation;

			public IReader Get(TypeInfo parameter) => new Reader(_activation.Get(parameter));

			sealed class Reader : IReader
			{
				readonly IReader    _reader;
				readonly IInstances _instances;

				public Reader(IReader reader) : this(reader, Instances.Default) {}

				public Reader(IReader reader, IInstances instances)
				{
					_reader    = reader;
					_instances = instances;
				}

				public object Get(IFormatReader parameter)
				{
					var key = parameter.Get()
					                   .AsValid<XmlReader>();

					var result = _instances.IsSatisfiedBy(key) ? _instances.Get(key) : _reader.Get(parameter);

					return result;
				}
			}
		}
	}
}