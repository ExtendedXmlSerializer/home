using System.Xml.Linq;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class Adapter<T> : IExtendedXmlCustomSerializer
	{
		readonly IExtendedXmlCustomSerializer<T> _configuration;

		public Adapter(IExtendedXmlCustomSerializer<T> configuration) => _configuration = configuration;

		public object Deserialize(XElement xElement) => _configuration.Deserialize(xElement);

		public void Serializer(XmlWriter xmlWriter, object instance)
			=> _configuration.Serializer(xmlWriter, instance.AsValid<T>());
	}

	/*sealed class CustomSerializers<T> : IParameterizedSource<Type, IExtendedXmlCustomSerializer<T>>
	{
		public static CustomSerializers<T> Default { get; } = new CustomSerializers<T>();
		CustomSerializers() : this(Activators.Default) {}

		readonly IActivators _activators;

		public CustomSerializers(IActivators activators) => _activators = activators;

		public IExtendedXmlCustomSerializer<T> Get(Type parameter) => _activators.New<IExtendedXmlCustomSerializer<T>>(parameter);
	}*/

/*
	public sealed class ActivatedXmlCustomSerializer<T, TSerializer> : IExtendedXmlCustomSerializer<T> where TSerializer : IExtendedXmlCustomSerializer<T>
	{
		public static ActivatedXmlCustomSerializer<T, TSerializer> Default { get; } = new ActivatedXmlCustomSerializer<T, TSerializer>();
		ActivatedXmlCustomSerializer() : this(CustomSerializers<T>.Default.Build(typeof(TSerializer))) {}

		readonly Func<IExtendedXmlCustomSerializer<T>> _source;

		public ActivatedXmlCustomSerializer(Func<IExtendedXmlCustomSerializer<T>> source) => _source = source;

		public T Deserialize(XElement xElement) => _source.Invoke().Deserialize(xElement);

		public void Serializer(XmlWriter xmlWriter, T instance)
		{
			_source.Invoke().Serializer(xmlWriter, instance);
		}
	}
*/
}