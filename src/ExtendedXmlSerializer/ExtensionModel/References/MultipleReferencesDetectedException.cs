using ExtendedXmlSerializer.ContentModel;

namespace ExtendedXmlSerializer.ExtensionModel.References;

sealed class MultipleReferencesDetectedException : ReferencesDetectedException
{
	public MultipleReferencesDetectedException(string message, IWriter writer) : base(message, writer) {}
}