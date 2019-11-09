using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel
{
	sealed class GenericSerializers : Generic<object, ISerializer>
	{
		public static GenericSerializers Default { get; } = new GenericSerializers();

		GenericSerializers() : base(typeof(GenericSerializerAdapter<>)) {}
	}
}