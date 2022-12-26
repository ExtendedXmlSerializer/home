using ExtendedXmlSerializer.ContentModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.References;

class ReferencesDetectedException : Exception
{
	public ReferencesDetectedException(string message, IWriter writer) : base(message) => Writer = writer;

	public IWriter Writer { get; }
}