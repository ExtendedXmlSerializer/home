// MIT License
//
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReflectionModel
{
	[SuppressMessage("ReSharper", "UnusedMember.Local")]
	[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
	public class MemberComparerTests
	{
		[Fact]
		public void Interface()
		{
			const string keys = nameof(IDictionary.Keys), values = nameof(IDictionary.Values);
			var definition = typeof(IDictionary<,>).GetRuntimeProperty(keys);

			var sut = new MemberComparer(ImplementedTypeComparer.Default);

			var constructed = typeof(IDictionary<string, int>);
			Assert.Equal(definition, constructed.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, constructed.GetRuntimeProperty(values), sut);

			var implementation = typeof(Dictionary<string, int>);
			Assert.Equal(definition, implementation.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, implementation.GetRuntimeProperty(values), sut);

			var not = typeof(NotDictionary<,>);
			Assert.NotEqual(definition, not.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, not.GetRuntimeProperty(values), sut);

			var notConstructed = typeof(NotDictionary<string, int>);
			Assert.NotEqual(definition, notConstructed.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, notConstructed.GetRuntimeProperty(values), sut);
		}


		[Fact]
		public void Definition()
		{
			const string keys = nameof(IDictionary.Keys), values = nameof(IDictionary.Values);
			var definition = typeof(Dictionary<,>).GetRuntimeProperty(keys);

			var sut = new MemberComparer(ImplementedTypeComparer.Default);

			var constructed = typeof(Dictionary<string, int>);
			Assert.Equal(definition, constructed.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, constructed.GetRuntimeProperty(values), sut);

			var not = typeof(NotDictionary<,>);
			Assert.NotEqual(definition, not.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, not.GetRuntimeProperty(values), sut);

			var notConstructed = typeof(NotDictionary<string, int>);
			Assert.NotEqual(definition, notConstructed.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, notConstructed.GetRuntimeProperty(values), sut);
		}

		[Fact]
		public void Explicit()
		{
			const string keys = nameof(IDictionary.Keys), values = nameof(IDictionary.Values);
			var definition = typeof(Dictionary<string, object>).GetRuntimeProperty(keys);

			var sut = new MemberComparer(TypeIdentityComparer.Default);

			var other = typeof(Dictionary<string, int>);
			Assert.NotEqual(definition, other.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, other.GetRuntimeProperty(values), sut);

			var implementation = typeof(Dictionary<string, object>);
			Assert.Equal(definition, implementation.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, implementation.GetRuntimeProperty(values), sut);

			var not = typeof(NotDictionary<,>);
			Assert.NotEqual(definition, not.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, not.GetRuntimeProperty(values), sut);

			var notConstructed = typeof(NotDictionary<string, int>);
			Assert.NotEqual(definition, notConstructed.GetRuntimeProperty(keys), sut);
			Assert.NotEqual(definition, notConstructed.GetRuntimeProperty(values), sut);
		}

		class NotDictionary<TKey, TValue>
		{
			public ICollection<TKey> Keys { get; }

			public ICollection<TValue> Values { get; }
		}
	}
}