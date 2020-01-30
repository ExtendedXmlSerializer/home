using ExtendedXmlSerializer.ContentModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class CircularReferencesDetectedException : Exception
	{
		public CircularReferencesDetectedException(string message, IWriter writer) : base(message) => Writer = writer;

		public IWriter Writer { get; }
	}
}