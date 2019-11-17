using System.Collections.Immutable;


namespace ExtendedXmlSerializer.Core
{

	/// <exclude />
	public class CompositeCommand<T> : ICommand<T>
	{
		readonly ImmutableArray<ICommand<T>> _items;

		/// <exclude />
		public CompositeCommand(params ICommand<T>[] items) : this(items.ToImmutableArray()) {}

		/// <exclude />
		public CompositeCommand(ImmutableArray<ICommand<T>> items) => _items = items;

		/// <exclude />
		public void Execute(T parameter)
		{
			foreach (var command in _items)
			{
				command.Execute(parameter);
			}
		}
	}
}