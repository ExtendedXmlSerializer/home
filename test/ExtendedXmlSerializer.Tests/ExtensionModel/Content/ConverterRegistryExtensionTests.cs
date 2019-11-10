using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Content
{
	public sealed class ConverterRegistryExtensionTests
	{
		[Fact]
		public void Boolean()
		{
			const bool subject = true;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Character()
		{
			const char subject = 'x';
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Byte()
		{
			const sbyte subject = -7;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void ByteArray()
		{
			var subject = new byte[] {6, 7, 7, 6};
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .BeEquivalentTo(subject);
		}

		[Fact]
		public void UnsignedByte()
		{
			byte subject = 7;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Short()
		{
			const ushort subject = 31000;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void UnsignedShort()
		{
			const short subject = -31000;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Integer()
		{
			const int subject = -6776;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void UnsignedInteger()
		{
			const uint subject = 6776;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Long()
		{
			const long subject = -6776677667766776;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void UnsignedLong()
		{
			const ulong subject = 6776677667766776;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Float()
		{
			const float subject = 1.23456f;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Double()
		{
			const double subject = 123456789123456789;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Decimal()
		{
			const decimal subject = 12345678912345678912;
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void DateTime()
		{
			var subject = new DateTime(2017, 10, 18);
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void DateTimeOffset()
		{
			DateTimeOffset subject = new DateTime(2017, 10, 18);
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void String()
		{
			const string subject = "Hello World!";
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Guid()
		{
			var subject = System.Guid.NewGuid();
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void TimeSpan()
		{
			var subject = System.TimeSpan.FromMilliseconds(6776);
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .Be(subject);
		}

		[Fact]
		public void Uri()
		{
			var subject = new Uri("https://extendedxmlserializer.github.io");
			new SerializationSupport().Cycle(subject)
			                          .Should()
			                          .BeEquivalentTo(subject);
		}
	}
}