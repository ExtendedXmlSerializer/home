using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue463Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = new ConfigurationContainer().Create().ForTesting();

			// my custom IDictionary<k,v> based type
			var instance = new DictionaryExtended {
				[0] = "Test0",
				[1] = "Test1",
				[2] = "Test2"
			};

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);

		}

		// Custom simple dictionary class.  The key thing seems to be:
				// this class inherits from IDictionary<int,string>
				// BUT it does NOT inherit from the non-generic IDictionary (the one with no types attached)
				//
				// this class was created minimally by deriving from IDictionary<int,string>
		// right click -> implement missing members by delegating to new object (dictionary).
		// it creates a bunch of members to implement to the interface
		// then add the initializer for the _dict
		public class DictionaryExtended : IDictionary<int, string>
		{
			IDictionary<int, string> _dict = new Dictionary<int, string>();

			public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
			{
				return _dict.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable) _dict).GetEnumerator();
			}

			public void Add(KeyValuePair<int, string> item)
			{
				_dict.Add(item);
			}

			public void Clear()
			{
				_dict.Clear();
			}

			public bool Contains(KeyValuePair<int, string> item)
			{
				return _dict.Contains(item);
			}

			public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
			{
				_dict.CopyTo(array, arrayIndex);
			}

			public bool Remove(KeyValuePair<int, string> item)
			{
				return _dict.Remove(item);
			}

			public int Count => _dict.Count;

			public bool IsReadOnly => _dict.IsReadOnly;

			public bool ContainsKey(int key)
			{
				return _dict.ContainsKey(key);
			}

			public void Add(int key, string value)
			{
				_dict.Add(key, value);
			}

			public bool Remove(int key)
			{
				return _dict.Remove(key);
			}

			public bool TryGetValue(int key, out string value)
			{
				return _dict.TryGetValue(key, out value);
			}

			public string this[int key]
			{
				get => _dict[key];
				set => _dict[key] = value;
			}

			public ICollection<int> Keys => _dict.Keys;

			public ICollection<string> Values => _dict.Values;
		}
	}
}
