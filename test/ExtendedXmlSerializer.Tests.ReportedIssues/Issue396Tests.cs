using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue396Tests
	{
		[Fact]
		public void NodesListIsMissingNodes()
		{
			var serializer = new ConfigurationContainer().UseOptimizedNamespaces()
			                                             .EnableParameterizedContentWithPropertyAssignments()
			                                             .UseAutoFormatting()
			                                             .Type<Node>()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();
			var node1 = new Node(1);
			var node2 = new Node(2).AddNode(node1, isBidirectional: true);
			var node3 = new Node(3).AddNode(node2, isBidirectional: false);
			node1.AddNode(node3, isBidirectional: false);
			var nodesList = new List<Node> { node1, node2, node3 };



			var cycled = serializer.Cycle(nodesList);
			cycled.Should().BeEquivalentTo(nodesList, config => config.IgnoringCyclicReferences());
		}

		public sealed class Node : IEquatable<Node>
		{
			private readonly List<Node> _linkedNodes;

			public Node(int id) : this(id, new List<Node>()) {}

			public Node(int id, List<Node> linkedNodes)
			{
				Id           = id;
				_linkedNodes = linkedNodes ?? new List<Node>();
			}

			public int Id { get; }

			public IReadOnlyList<Node> LinkedNodes => _linkedNodes;

			// ReSharper disable once FlagArgument
			public Node AddNode(Node otherNode, bool isBidirectional = true)
			{
				if (otherNode == null)
					throw new ArgumentNullException(nameof(otherNode));
				if (ReferenceEquals(this, otherNode))
					throw new ArgumentException($"You cannot link a node {Id} to itself.");

				_linkedNodes.Add(otherNode);
				if (isBidirectional)
					otherNode.AddNode(this, false);
				return this;
			}

			public bool Equals(Node other) => !ReferenceEquals(null, other) && Id == other.Id;

			public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Node other && Equals(other);

			public override int GetHashCode() => Id;

			public override string ToString() => "Node " + Id;
		}
	}
}