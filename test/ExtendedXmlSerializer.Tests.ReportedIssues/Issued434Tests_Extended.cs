using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issued434Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			/*var serializer = new ConfigurationContainer().EnableParameterizedContentWithPropertyAssignments()
			                                             .Type<Parent>()
			                                             .EnableReferences(node => node.Id)
			                                             .Type<Child>()
			                                             .EnableReferences(nodeLink => nodeLink.Id)
			                                             .Create().ForTesting();

			var parent = new Parent();
			parent.Children.Add(new Child(parent));

			var cycled = serializer.Cycle(parent);
			Debugger.Break();*/

		}


		sealed class Parent
		{
			public Parent() : this(Guid.NewGuid()) {}

			public Parent(Guid id) => Id = id;

			[UsedImplicitly]
			public Guid Id { get; }

			[UsedImplicitly]
			public List<Child> Children { get; set; } = new List<Child>();
		}

		sealed class Child
		{
			public Child(Parent parent) : this(Guid.NewGuid(), parent) {}

			public Child(Guid id, Parent parent)
			{
				Id     = id;
				Parent = parent;
			}

			[UsedImplicitly]
			public Guid Id { get; }
			public Parent Parent { [UsedImplicitly] get; }
		}
	}
}
