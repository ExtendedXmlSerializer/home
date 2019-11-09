using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReflectionModel
{
	[SuppressMessage("ReSharper", "UnusedMember.Local"), SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
	public class MemberComparerTests
	{
		[Fact]
		public void Interface()
		{
			const string keys       = nameof(IDictionary.Keys), values = nameof(IDictionary.Values);
			var          definition = typeof(IDictionary<,>).GetRuntimeProperty(keys);

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
			const string keys       = nameof(IDictionary.Keys), values = nameof(IDictionary.Values);
			var          definition = typeof(Dictionary<,>).GetRuntimeProperty(keys);

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
			const string keys       = nameof(IDictionary.Keys), values = nameof(IDictionary.Values);
			var          definition = typeof(Dictionary<string, object>).GetRuntimeProperty(keys);

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