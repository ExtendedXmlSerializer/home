using ExtendedXmlSerializer.ContentModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class CircularReferencesDetectedException : ReferencesDetectedException
	{
		public CircularReferencesDetectedException(string message, IWriter writer) : base(message, writer) {}
	}

	class ReferencesDetectedException : Exception
	{
		public ReferencesDetectedException(string message, IWriter writer) : base(message) => Writer = writer;

		public IWriter Writer { get; }
	}

	// TODO

	sealed class MultipleReferencesDetectedException : ReferencesDetectedException
	{
		public MultipleReferencesDetectedException(string message, IWriter writer) : base(message, writer) {}
	}
}