using ExtendedXmlSerializer.ContentModel.Conversion;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	/// <summary>
	/// Converter for <see cref="Version"/> instances.
	/// </summary>
	public sealed class VersionConverter : Converter<Version>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static VersionConverter Default { get; } = new VersionConverter();

		VersionConverter() : base(x => new Version(x), x => x.ToString()) {}
	}
}