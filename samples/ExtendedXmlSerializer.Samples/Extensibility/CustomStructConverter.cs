using ExtendedXmlSerializer.ContentModel.Conversion;
using System.Reflection;

namespace ExtendedXmlSerializer.Samples.Extensibility
{
	// CustomConverter
	public sealed class CustomStructConverter : IConverter<CustomStruct>
	{
		public static CustomStructConverter Default { get; } = new CustomStructConverter();
		CustomStructConverter() {}

		public bool IsSatisfiedBy(TypeInfo parameter) => typeof(CustomStruct).GetTypeInfo()
		                                                                     .IsAssignableFrom(parameter);

		public CustomStruct Parse(string data) =>
			int.TryParse(data, out var number) ? new CustomStruct(number) : CustomStruct.Default;

		public string Format(CustomStruct instance) => instance.Number.ToString();
	}

	public struct CustomStruct
	{
		public static CustomStruct Default { get; } = new CustomStruct(6776);

		public CustomStruct(int number)
		{
			Number = number;
		}
		public int Number { get; }
	}
	// EndCustomConverter
}