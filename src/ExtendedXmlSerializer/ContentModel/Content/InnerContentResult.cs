namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class InnerContentResult : IInnerContentResult
	{
		public static InnerContentResult Default { get; } = new InnerContentResult();

		InnerContentResult() {}

		public object Get(IInnerContent parameter) => parameter.Current;
	}
}