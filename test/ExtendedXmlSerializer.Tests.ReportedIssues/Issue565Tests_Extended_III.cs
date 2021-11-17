using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue565Tests_Extended_III
	{
		[Fact]
		public void Verify()
		{
			var instances = new[]
			{
				new Model
				{
					Key = new List<byte[]>
					{
						Encoding.UTF8.GetBytes("Hello"),
						Encoding.UTF8.GetBytes("World"),
					},
					Message = "First"
				},
				new Model
				{
					Key = new List<byte[]>
					{
						Encoding.UTF8.GetBytes("Hello1"),
						Encoding.UTF8.GetBytes("World1"),
					},
					Message = "Second"
				},
				new Model
				{
					Key = new List<byte[]>
					{
						Encoding.UTF8.GetBytes("Hello2"),
						Encoding.UTF8.GetBytes("World2"),
					},
					Message = "Third"
				},
				new Model
				{
					Key = new List<byte[]>
					{
						Encoding.UTF8.GetBytes("Hello3"),
						Encoding.UTF8.GetBytes("World3"),
					},
					Message = "Fourth"
				},
				new Model
				{
					Key = new List<byte[]>
					{
						Encoding.UTF8.GetBytes("Hello4"),
						Encoding.UTF8.GetBytes("World4"),
					},
					Message = "Fifth"
				},
				new Model
				{
					Key = new List<byte[]>
					{
						Encoding.UTF8.GetBytes("Hello5"),
						Encoding.UTF8.GetBytes("World5"),
					},
					Message = "Sixth"
				}
			};
			var instance = new Store();
			foreach (var model in instances)
			{
				instance.Add(model.Key, model);
			}
			var subject  = new ConfigurationContainer().EnableImplicitTyping(instance.GetType()).Create().ForTesting();
			var cycle    = subject.Cycle(instance);
			cycle.Values.Should().BeEquivalentTo(instance.Values);
			cycle.Should().BeEquivalentTo(instance);
			cycle.Count.Should().Be(instance.Count);
		}

		public sealed class Model
		{
			public List<byte[]> Key { get; set; }

			public string Message { get; set; }
		}

		sealed class Store : Dictionary<List<byte[]>, Model>
		{
			public Store() : base(Issue565Tests_Extended_III.Comparer.Instance) {}
		}

		sealed class Comparer : EqualityComparer<List<byte[]>>
		{
			public static Comparer Instance { get; } = new();

			Comparer() {}

			public override bool Equals(List<byte[]> x, List<byte[]> y)
				=> (x != null ? string.Join(string.Empty, x.Select(Encoding.UTF8.GetString)) : string.Empty)
				   ==
				   (y != null ? string.Join(string.Empty, y.Select(Encoding.UTF8.GetString)) : string.Empty);

			public override int GetHashCode(List<byte[]> obj)
				=> string.Join(string.Empty, obj.Select(Encoding.UTF8.GetString)).GetHashCode();
		}
	}
}