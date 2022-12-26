using ExtendedXmlSerializer.ContentModel;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class CircularReferencesDetectedException : ReferencesDetectedException
	{
		public CircularReferencesDetectedException(string message, IWriter writer) : base(message, writer) {}
	}
}