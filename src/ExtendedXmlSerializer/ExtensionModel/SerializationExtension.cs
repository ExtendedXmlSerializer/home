using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ISerializer = ExtendedXmlSerializer.ExtensionModel.Xml.ISerializer;
using Serializer = ExtendedXmlSerializer.ExtensionModel.Xml.Serializer;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// A default extension that registers all necessary and fundamental components required for successful serialization
	/// deserialization.
	/// </summary>
	public sealed class SerializationExtension : ISerializerExtension
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static SerializationExtension Default { get; } = new SerializationExtension();

		SerializationExtension() {}

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register<IRead, Read>()
			            .Register<IWrite, Write>()
			            .Register<ISerializer, Serializer>()
			            .Register<IRuntimeSerialization, RuntimeSerialization>()
			            .Register<ContentModel.ISerializer, RuntimeSerializer>()
			            .Register<RuntimeSerializers>()
			            .Register<ISerializers, Serializers>()
			            .RegisterInstance(RuntimeSerializationExceptionMessage.Default)
			            .Decorate<ISerializers, NullableAwareSerializers>()
			            .Decorate<ISerializers, ReferenceAwareSerializers>()
			            .Decorate<IContents, RecursionAwareContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}