using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue218Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new MyListImpl<string>("Hello World!");
			new ConfigurationContainer().EnableParameterizedContent()
			                            .Create()
			                            .Cycle(instance)
			                            .ShouldBeEquivalentTo(instance);
		}

		public class MyListImpl<T> : List<T>
		{
			public MyListImpl(object owner) => Owner = owner;

			public object Owner { get; }
		}
	}
}