namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RecursionContents : IRecursionContents
	{
		public static RecursionContents Default { get; } = new RecursionContents();

		RecursionContents() {}

		public IContents Get(IContents parameter) => new Recursion(parameter);
	}
}