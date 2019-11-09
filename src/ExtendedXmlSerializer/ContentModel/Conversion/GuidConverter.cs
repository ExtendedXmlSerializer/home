using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	class GuidConverter : Converter<Guid>
	{
		public static GuidConverter Default { get; } = new GuidConverter();

		GuidConverter() : base(XmlConvert.ToGuid, XmlConvert.ToString) {}
	}
}