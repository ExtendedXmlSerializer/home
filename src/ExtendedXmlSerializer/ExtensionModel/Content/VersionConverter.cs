using System;
using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class VersionConverter : Converter<Version>
	{
		public static VersionConverter Default { get; } = new VersionConverter();

		VersionConverter() : base(x => new Version(x), x => x.ToString()) {}
	}
}