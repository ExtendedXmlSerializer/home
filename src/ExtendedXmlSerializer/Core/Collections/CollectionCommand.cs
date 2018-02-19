using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.Core.Collections
{
	public interface IElements<T> : IMembership<T>, IEnumerable<T> {}

	public interface IMembership<in T>
	{
		ICommand<T> Add { get; }

		ICommand<T> Remove { get; }
	}

	class Membership<T> : IMembership<T>
	{
		public Membership(ICollection<T> collection) : this(new AddCommand<T>(collection), new RemoveCommand<T>(collection)) {}

		public Membership(ICommand<T> add, ICommand<T> remove)
		{
			Add    = add;
			Remove = remove;
		}

		public ICommand<T> Add { get; }

		public ICommand<T> Remove { get; }
	}

	public interface IListings<T> : IElements<T>
	{
		ICommand<Insert<T>> Insert { get; }
	}

	public struct Insert<T>
	{
		public Insert(T element, int index)
		{
			Element = element;
			Index   = index;
		}

		public T Element { get; }

		public int Index { get; }
	}

	class Elements<T> : Membership<T>, IElements<T>
	{
		readonly IEnumerable<T> _items;

		public Elements(ICollection<T> collection) : this(collection, new AddCommand<T>(collection),
		                                                  new RemoveCommand<T>(collection)) {}

		public Elements(IEnumerable<T> items, ICommand<T> add, ICommand<T> remove) : base(add, remove) => _items = items;

		public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
