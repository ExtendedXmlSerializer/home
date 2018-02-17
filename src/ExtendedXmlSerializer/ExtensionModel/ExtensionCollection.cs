using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ExtensionCollection : IList<ISerializerExtension>
	{
		readonly ICollection<ISerializerExtension> _container;
		readonly IList<ISerializerExtension>       _collection;

		[UsedImplicitly]
		public ExtensionCollection(ICollection<ISerializerExtension> container, params ISerializerExtension[] items)
			: this(container.AddingAll(items), new List<ISerializerExtension>(items)) {}

		public ExtensionCollection(ICollection<ISerializerExtension> container, IList<ISerializerExtension> collection)
		{
			_container  = container;
			_collection = collection;
		}

		public IEnumerator<ISerializerExtension> GetEnumerator() => _collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(ISerializerExtension item)
		{
			_container.Add(item);
			_collection.Add(item);
		}

		public void Clear()
		{
			foreach (var extension in _collection)
			{
				_container.Remove(extension);
			}
			_collection.Clear();
		}

		public bool Contains(ISerializerExtension item) => _collection.Contains(item);

		public void CopyTo(ISerializerExtension[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		public bool Remove(ISerializerExtension item) => _collection.Remove(item) && _container.Remove(item);

		public int Count => _collection.Count;

		public bool IsReadOnly => _collection.IsReadOnly;
		public int IndexOf(ISerializerExtension item)
		{
			_container.Add(item);
			return _collection.IndexOf(item);
		}

		public void Insert(int index, ISerializerExtension item)
		{
			_container.Add(item);
			_collection.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_container.Remove(_collection[index]);
			_collection.RemoveAt(index);
		}

		public ISerializerExtension this[int index]
		{
			get => _collection[index];
			set => _collection[index] = value;
		}
	}
}