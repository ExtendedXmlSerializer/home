using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue217Tests
	{
		[Fact]
		void Verify()
		{
			new ConfigurationContainer().Type<ChildClass>()
			                            .EmitWhen(x => x.NeedSerialize)
			                            .Create()
			                            .ForTesting()
			                            .Assert(new ParentClass(),
			                                    @"<?xml version=""1.0"" encoding=""utf-8""?><Issue217Tests-ParentClass xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Childs><Capacity>4</Capacity><Issue217Tests-ChildClass><NeedSerialize>true</NeedSerialize></Issue217Tests-ChildClass></Childs></Issue217Tests-ParentClass>");
		}

		public class ParentClass
		{
			public ParentClass()
				=> Childs = new List<ChildClass> {new ChildClass(), new ChildClass {NeedSerialize = true}};

			public List<ChildClass> Childs { [UsedImplicitly] get; set; }
		}

		public class ChildClass
		{
			public bool NeedSerialize { get; set; }
		}
	}
}