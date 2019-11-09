namespace ExtendedXmlSerializer.Core.Sources
{
	public sealed class AlterationAdapter<T> : IAlteration<object>
	{
		readonly IAlteration<T> _alteration;

		public AlterationAdapter(IAlteration<T> alteration)
		{
			_alteration = alteration;
		}

		public object Get(object parameter) => _alteration.Get(parameter.To<T>());
	}
}