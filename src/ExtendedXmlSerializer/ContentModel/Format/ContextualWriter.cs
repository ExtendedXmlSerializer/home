using System;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class ContextualWriter<T> : IWriter<T>
	{
		readonly Func<IFormatWriter, T, string> _formatter;
		readonly IIdentity                      _identity;

		public ContextualWriter(Func<IFormatWriter, T, string> formatter, IIdentity identity)
		{
			_formatter = formatter;
			_identity  = identity;
		}

		public void Write(IFormatWriter writer, T instance) => writer.Content(_identity, _formatter(writer, instance));
	}
}