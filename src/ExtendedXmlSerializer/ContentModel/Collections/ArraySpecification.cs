using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class ArraySpecification : CollectionSpecification
	{
		public static ArraySpecification Default { get; } = new ArraySpecification();

		ArraySpecification() : base(IsArraySpecification.Default) {}
	}
}