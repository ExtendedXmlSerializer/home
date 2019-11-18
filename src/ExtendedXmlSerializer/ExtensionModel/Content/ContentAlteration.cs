using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public readonly struct ContentAlteration
	{
		public ContentAlteration(IAlteration<object> read, IAlteration<object> write)
		{
			Read  = read;
			Write = write;
		}

		public IAlteration<object> Read { get; }

		public IAlteration<object> Write { get; }
	}
}