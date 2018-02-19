namespace ExtendedXmlSerializer.Core.Collections
{
	public static class ExtensionMethods
	{
		public static IMembership<T> Execute<T>(this IMembership<T> @this, T parameter)
			=> @this.Add.Executed(parameter).Return(@this);
	}
}
