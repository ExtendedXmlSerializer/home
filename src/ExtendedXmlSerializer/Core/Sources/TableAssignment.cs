namespace ExtendedXmlSerializer.Core.Sources
{
	class TableAssignment<TKey, TValue> : ICommand<TValue>
	{
		readonly ITableSource<TKey, TValue> _source;
		readonly TKey                       _key;

		public TableAssignment(TKey key, ITableSource<TKey, TValue> source)
		{
			_key    = key;
			_source = source;
		}

		public void Execute(TValue parameter) => _source.Assign(_key, parameter);
	}
}