using System;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	sealed class ContextualReader<T> : IReader<T>
	{
		readonly Func<IFormatReader, T> _context;
		readonly IIdentity              _identity;

		public ContextualReader(Func<IFormatReader, T> context, IIdentity identity)
		{
			_context  = context;
			_identity = identity;
		}

		public T Get(IFormatReader parameter) => parameter.IsSatisfiedBy(_identity) ? _context(parameter) : default;
	}
}