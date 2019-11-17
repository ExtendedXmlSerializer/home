namespace ExtendedXmlSerializer.ContentModel.Content
{
	/// <summary>
	/// Configures a type member to wrap its contents in a CDATA container.
	/// </summary>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/150"/>
	public sealed class VerbatimAttribute : ContentSerializerAttribute
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public VerbatimAttribute() : base(typeof(VerbatimContentSerializer)) {}
	}
}