using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using Xunit;
using Xunit.Abstractions;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue662Tests 
	{
        readonly ITestOutputHelper _output;

        public Issue662Tests(ITestOutputHelper output)
        {
            _output = output;
        }
			
        [Fact]
		public void Verify()
		{
            int targget = 750;

            Node head = new Node();
            Node current = head;
			Node repeted = new Node()
			{
				Value = 123,
				Childes = new List<Node>()
				{
					new Node() { Value = 127 }
				}
			};
            for (int i = 1; i < targget; i++)
            {
                current.Childes.Add(current = new Node() { Value = i });
                current.Childes.Add(new Node() { Value = i + 1 });
                current.Childes.Add(new Node() { Value = i + 2 });
				current.Childes.Add(repeted);
            }

            var sut = GetDefaultConfig().UseHeap()
                                        .Create()
                                        .ForTesting();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            var serialized = sut.Serialize(settings, head);
            _output.WriteLine("actual:");
            _output.WriteLine(serialized);
            if (targget < 500)
            {
                var classic = GetDefaultConfig().Create()
												.ForTesting();
                var excepted = classic.Serialize(settings, head);
                _output.WriteLine("excepted:");
                _output.WriteLine(excepted);
                Assert.Equal(excepted, serialized);
            }

            var deserialized = sut.Deserialize<Node>(serialized);

            if (targget < 200)
                Assert.Equal(head, deserialized, new NodeEqualityComparer());

            IConfigurationContainer GetDefaultConfig() =>
                  new ConfigurationContainer()
										      .EnableAllConstructors()
                                              .UseAutoFormatting()
                                              .EnableXmlText()
                                              .EnableClassicListNaming()
                                              .UseOptimizedNamespaces()
                                              .AllowMultipleReferences()
                                              .EnableMemberExceptionHandling()
                                              ;
        }

        public class Node
        {
            public List<Node> Childes { get; set; } = new List<Node>();

            public int Value { get; set; }
        }

        public class NodeEqualityComparer : IEqualityComparer<Node>
        {
			public bool Equals(Node x, Node y)
			{
				if (ReferenceEquals(null, x)) return ReferenceEquals(null, y);
				if (ReferenceEquals(x, y)) return true;

				var stack = new Stack<(Node a, Node b)>();
				var visited = new HashSet<(Node a, Node b)>(new ReferencePairComparer());

				stack.Push((x, y));

				while (stack.Count > 0)
				{
					var (a, b) = stack.Pop();

					if (ReferenceEquals(a, b)) continue;
					if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

					if (a.Value != b.Value) return false;

					var ca = a.Childes;
					var cb = b.Childes;

					if (ReferenceEquals(ca, cb)) continue;
					if (ca == null || cb == null) return false;
					if (ca.Count != cb.Count) return false;

					if (!visited.Add((a, b))) continue;

					for (int i = 0; i < ca.Count; i++)
					{
						stack.Push((ca[i], cb[i]));
					}
				}

				return true;
			}

			public int GetHashCode(Node obj)
			{
				unchecked
				{
					var hashCode = 47;
					if (obj.Childes != null)
						hashCode = (hashCode * 53) ^ EqualityComparer<List<Node>>.Default.GetHashCode(obj.Childes);

					hashCode = (hashCode * 53) ^ obj.Value.GetHashCode();
					return hashCode;
				}
			}

			// do not care the order of the pair
			sealed class ReferencePairComparer : IEqualityComparer<(Node a, Node b)>
			{
				public bool Equals((Node a, Node b) x, (Node a, Node b) y)
				{
					if (ReferenceEquals(x.a, y.a) && ReferenceEquals(x.b, y.b))
						return true;

					return ReferenceEquals(x.a, y.b) && ReferenceEquals(x.b, y.a);
                }

				public int GetHashCode((Node a, Node b) obj)
				{
                    int h1 = RuntimeHelpers.GetHashCode(obj.a);
					int h2 = RuntimeHelpers.GetHashCode(obj.b);
					if (h1 < h2)
					{
						var tmp = h1;
						h1 = h2;
						h2 = tmp;
					}
					unchecked
					{
						return (h1 * 397) ^ h2;
					}
				}
			}
		}

    }
}
