using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	public interface IServiceList : IImmutableList<object>, IServiceProvider {}

		class ServiceList : IServiceList
	{
		readonly IImmutableList<object> _services;

		public ServiceList(IImmutableList<object> services)
		{
			_services = services;
		}

		public IEnumerator<object> GetEnumerator() => _services.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _services.GetEnumerator();

		public int Count => _services.Count;

		public object this[int index] => _services[index];

		public IImmutableList<object> Clear() => _services.Clear();

		public IImmutableList<object> Add(object value) => _services.Add(value);

		public IImmutableList<object> Replace(object oldValue, object newValue, IEqualityComparer<object> equalityComparer)
			=> _services.Replace(oldValue, newValue, equalityComparer);

		public IImmutableList<object> SetItem(int index, object value) => _services.SetItem(index, value);

		public IImmutableList<object> RemoveAt(int index) => _services.RemoveAt(index);

		public IImmutableList<object> RemoveRange(int index, int count) => _services.RemoveRange(index, count);

		public IImmutableList<object> RemoveRange(IEnumerable<object> items, IEqualityComparer<object> equalityComparer)
			=> _services.RemoveRange(items, equalityComparer);

		public IImmutableList<object> RemoveAll(Predicate<object> match) => _services.RemoveAll(match);

		public IImmutableList<object> Remove(object value, IEqualityComparer<object> equalityComparer)
			=> _services.Remove(value, equalityComparer);

		public IImmutableList<object> InsertRange(int index, IEnumerable<object> items) => _services.InsertRange(index, items);

		public IImmutableList<object> Insert(int index, object element) => _services.Insert(index, element);

		public IImmutableList<object> AddRange(IEnumerable<object> items) => _services.AddRange(items);

		public int LastIndexOf(object item, int index, int count, IEqualityComparer<object> equalityComparer)
			=> _services.LastIndexOf(item, index, count, equalityComparer);

		public int IndexOf(object item, int index, int count, IEqualityComparer<object> equalityComparer)
			=> _services.IndexOf(item, index, count, equalityComparer);

		public object GetService(Type serviceType) => new ServiceProvider(_services.ToImmutableArray()).GetService(serviceType);
	}

}