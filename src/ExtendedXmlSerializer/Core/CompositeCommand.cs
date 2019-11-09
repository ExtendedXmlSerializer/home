using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core
{
	public class CompositeCommand<T> : ICommand<T>
	{
		readonly ImmutableArray<ICommand<T>> _items;

		public CompositeCommand(params ICommand<T>[] items) : this(items.ToImmutableArray()) {}

		public CompositeCommand(ImmutableArray<ICommand<T>> items) => _items = items;

		public void Execute(T parameter)
		{
			foreach (var command in _items)
			{
				command.Execute(parameter);
			}
		}
	}
}