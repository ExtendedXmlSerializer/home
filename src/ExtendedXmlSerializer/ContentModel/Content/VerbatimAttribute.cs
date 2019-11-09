namespace ExtendedXmlSerializer.ContentModel.Content
{
	public sealed class VerbatimAttribute : ContentSerializerAttribute
	{
		public VerbatimAttribute() : base(typeof(VerbatimContentSerializer)) {}
	}
}