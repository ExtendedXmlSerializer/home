using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ThreadProtectionExtension : ISerializerExtension
	{
		public static ThreadProtectionExtension Default { get; } = new ThreadProtectionExtension();

		ThreadProtectionExtension() {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<ISerializer, Serializer>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Serializer : ISerializer
		{
			readonly ISerializer _serializer;

			public Serializer(ISerializer serializer) => _serializer = serializer;

			public object Deserialize(System.Xml.XmlReader reader)
			{
				lock (_serializer)
				{
					return _serializer.Deserialize(reader);
				}
			}

			public void Serialize(System.Xml.XmlWriter writer, object instance)
			{
				lock (_serializer)
				{
					_serializer.Serialize(writer, instance);
				}
			}
		}
	}
}