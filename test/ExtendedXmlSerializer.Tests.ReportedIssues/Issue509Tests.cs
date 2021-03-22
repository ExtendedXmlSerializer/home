using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue509Tests
	{
		[Fact]
		public void Verify()
		{
			true.Should().BeTrue();

			var serializer = new ConfigurationContainer().Type<Key>()
			                                             .Register()
			                                             .Converter()
			                                             .Using(KeyConverter.Default)
			                                             .Create()
			                                             .ForTesting();
			var key = new Key(6776);
			serializer.Cycle(key).Should().BeEquivalentTo(key);
		}

		[Fact]
		public void VerifyTable()
		{
			var serializer = new ConfigurationContainer().Type<Key>()
			                                             .Register()
			                                             .Converter()
			                                             .Using(KeyConverter.Default)
			                                             .UseAutoFormatting()
			                                             .Create()
			                                             .ForTesting();

			var instance = new KeyDictionary<string> { { 6776, "Hello World!" } };

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		sealed class KeyDictionary<T> : Dictionary<Key, T>
		{
			public KeyDictionary() : base(KeyEquality.Default) {}
		}

		sealed class KeyEquality : IEqualityComparer<Key>
		{
			public static KeyEquality Default { get; } = new KeyEquality();

			KeyEquality() {}

			public bool Equals(Key x, Key y) => x.Value == y.Value;

			public int GetHashCode(Key obj) => obj.Value;
		}

		sealed class KeyConverter : Converter<Key>
		{
			public static KeyConverter Default { get; } = new KeyConverter();

			KeyConverter() : base(KeyConvert.Default.Parse, KeyConvert.Default.Format) {}
		}

		sealed class KeyConvert : IConvert<Key>
		{
			public static KeyConvert Default { get; } = new KeyConvert();

			KeyConvert() {}

			public Key Parse(string data) => int.Parse(data, NumberStyles.HexNumber | NumberStyles.AllowHexSpecifier);

			public string Format(Key instance) => instance.ToString();
		}

		public readonly struct Key
		{
			public static implicit operator int(Key key) => key.Value;

			public static implicit operator Key(int value) => new Key(value);

			public Key(int value) => Value = value;

			public int Value { get; }

			public override string ToString() => Value.ToString("X");
		}
	}
}