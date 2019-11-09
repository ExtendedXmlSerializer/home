using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class SerializationExtension : ISerializerExtension
	{
		public static SerializationExtension Default { get; } = new SerializationExtension();

		SerializationExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Register(typeof(IRead<>), typeof(Read<>))
			            .Register(typeof(IWrite<>), typeof(Write<>))
			            .Register(typeof(ISerializer<,>), typeof(Serializer<,>))
			            .Register<IRuntimeSerialization, RuntimeSerialization>()
			            .Register<ISerializer, RuntimeSerializer>()
			            .Register<RuntimeSerializers>()
			            .Register<ISerializers, Serializers>()
			            .RegisterInstance(RuntimeSerializationExceptionMessage.Default)
			            .Decorate<ISerializers, NullableAwareSerializers>()
			            .Decorate<ISerializers, ReferenceAwareSerializers>()
			            .Decorate<IContents, RecursionAwareContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}