using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class StringConverter : Converter<string>
	{
		readonly static Func<string, string> Self = Self<string>.Default.Get;

		public static StringConverter Default { get; } = new StringConverter();

		StringConverter() : base(Self, Self) {}
	}
}