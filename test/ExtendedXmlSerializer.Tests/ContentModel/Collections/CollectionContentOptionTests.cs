using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Collections
{
	public class CollectionContentOptionTests
	{
		[Fact]
		public void Field()
		{
			var support  = new SerializationSupport();
			var expected = new Subject("Hello", "World!");
			var actual =
				support.Assert(expected,
				               @"<?xml version=""1.0"" encoding=""utf-8""?><CollectionContentOptionTests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Collections;assembly=ExtendedXmlSerializer.Tests""><_children><Capacity>4</Capacity><string xmlns=""https://extendedxmlserializer.github.io/system"">Hello</string><string xmlns=""https://extendedxmlserializer.github.io/system"">World!</string></_children></CollectionContentOptionTests-Subject>");
			Assert.Equal(expected.ToArray(), actual.ToArray());
		}

		class Subject : IEnumerable<string>
		{
			[XmlElement] readonly List<string> _children = new List<string>();

			public Subject(params string[] items)
			{
				_children.AddRange(items);
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IEnumerator<string> GetEnumerator() => _children.GetEnumerator();
		}
	}
}