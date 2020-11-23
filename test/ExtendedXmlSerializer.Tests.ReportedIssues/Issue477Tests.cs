using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using Xunit;

#nullable enable
namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue477Tests
	{
		[Fact]
		public void SerializeAndDeserializeInstanceA()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
			                                             .Create()
			                                             .ForTesting();

			var instance = new A { Point = new Point(2, 3) };

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void SerializeAndDeserializeInstanceB()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .Create()
			                                             .ForTesting();

			var instance = new B { NullablePoint = new Point(24, 7) };

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		[Fact]
		public void SerializeAndDeserializeInstanceBNull()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .Create()
			                                             .ForTesting();

			var instance = new B { NullablePoint = null };

			serializer.Assert(instance, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue477Tests-B xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"" />");

			serializer.Cycle(instance).Should().BeEquivalentTo(instance);
		}

		public sealed class A
		{
			public Point Point { get; set; }
		}

		public sealed class B
		{
			public Point? NullablePoint { get; set; }
		}

		public readonly struct Point : IEquatable<Point>
		{
			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}

			public int X { get; }

			public int Y { get; }

			public bool Equals(Point other) => X == other.X && Y == other.Y;

			public override bool Equals(object? obj) => obj is Point other && Equals(other);

			public override int GetHashCode()
			{
				unchecked
				{
					return (X * 397) ^ Y;
				}
			}

			public override string ToString() => $"({X}, {Y})";

			public static bool operator ==(Point left, Point right) => left.Equals(right);

			public static bool operator !=(Point left, Point right) => !left.Equals(right);
		}
	}
}
#nullable restore