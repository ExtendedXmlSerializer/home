#nullable enable
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue434Tests
	{
		[Fact]
		public void Verify()
		{
			/*var serializer = CreateDefaultSerializer().ForTesting();

			var node = new Node(Guid.NewGuid());
			var link = new NodeLink(Guid.NewGuid()) {Node1 = node};
			node.NodeLinks.Add(link);
			serializer.WriteLine(node);
			var cycled = serializer.Cycle(node);
			Debugger.Break();*/
		}

		[Fact]
		public void EndlessRecursionOnSecondCallToSerialize()
		{
			var serializer = CreateDefaultSerializer().ForTesting();

			var map = new Map();

			var node1 = map.AddEntity(new Node(Guid.NewGuid()));
			var node2 = map.AddEntity(new Node(Guid.NewGuid()));
			map.AddEntity(NodeLink.CreateAttachedNodeLink(node1, node2));

			serializer.Serialize(map).Should().NotBeEmpty();

			var node3 = map.AddEntity(new Node(Guid.NewGuid()));
			var node4 = map.AddEntity(new Node(Guid.NewGuid()));
			map.AddEntity(NodeLink.CreateAttachedNodeLink(node3, node4));

			serializer.Serialize(map).Should().NotBeEmpty();
		}

		private static IExtendedXmlSerializer CreateDefaultSerializer() =>
			new ConfigurationContainer() //.UseOptimizedNamespaces()
				.EnableParameterizedContentWithPropertyAssignments()
				//.UseAutoFormatting()
				.Type<Node>()
				.EnableReferences(node => node.Id)
				.Type<NodeLink>()
				.EnableReferences(nodeLink => nodeLink.Id)
				.Create();

		public abstract class Entity : IEquatable<Entity>
		{
			protected Entity(Guid id)
			{
				Id = id;
			}

			public Guid Id { get; }

			public bool Equals(Entity? other)
			{
				if (ReferenceEquals(this, other))
					return true;
				if (other is null)
					return false;

				return Id == other.Id;
			}

			public override bool Equals(object? obj) =>
				obj is Entity entity && Equals(entity);

			public override int GetHashCode() => Id.GetHashCode();
		}

		public sealed class Map
		{
			private List<Node>?     _nodes;
			private List<NodeLink>? _nodeLinks;

			public List<Node> Nodes
			{
				get => _nodes ??= new List<Node>();
				set => _nodes = value;
			}

			public List<NodeLink> NodeLinks
			{
				get => _nodeLinks ??= new List<NodeLink>();
				set => _nodeLinks = value;
			}

			public T AddEntity<T>(T entity)
				where T : Entity
			{
				switch (entity)
				{
					case Node node:
						Nodes.Add(node);
						break;
					case NodeLink nodeLink:
						NodeLinks.Add(nodeLink);
						break;
				}

				return entity;
			}
		}

		public sealed class Node : Entity
		{
			private List<NodeLink>? _nodeLinks;

			public Node(Guid id) : base(id) {}

			public List<NodeLink> NodeLinks
			{
				get => _nodeLinks ??= new List<NodeLink>();
				set => _nodeLinks = value;
			}

			public void AddNodeLink(NodeLink nodeLink)
			{
				if (!ReferenceEquals(this, nodeLink.Node1) && !ReferenceEquals(this, nodeLink.Node2))
					throw new
						ArgumentException($"Node {Id} is not referenced from the node link {nodeLink.Id}, yet you want to attach it to this node.",
						                  nameof(nodeLink));

				NodeLinks.Add(nodeLink);
			}
		}

		public sealed class NodeLink : Entity
		{
			public NodeLink(Guid id) : base(id) {}

			public Node? Node1 { get; set; }

			public Node? Node2 { get; set; }

			public static NodeLink CreateAttachedNodeLink(Node node1, Node node2)
			{
				var nodeLink = new NodeLink(Guid.NewGuid()) {Node1 = node1, Node2 = node2};
				node1.AddNodeLink(nodeLink);
				node2.AddNodeLink(nodeLink);
				return nodeLink;
			}
		}
	}
}
#nullable restore