using System;
using ExtendedXmlSerializer.ContentModel;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	class CircularReferencesDetectedException : Exception
	{
		public CircularReferencesDetectedException(string message, IWriter writer) : base(message)
		{
			Writer = writer;
		}

		public IWriter Writer { get; }
	}
}