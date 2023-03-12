using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue599Tests
	{
		[Fact]
		public void Verify()
		{
			var sut = new ConfigurationContainer().AllowTargetInstances().Create().ForTesting();

			var instance = new NewDefinitionView
			{
				Name            = "Hello world",
				Description     = "Does this work?",
				DescriptionFull = "This is a longer description",
				Average         = 12345,
				Market          = MarketClassification.Traditional,
				Result          = Guid.NewGuid(),
				Price           = 123.21M,
				Identifier      = "unique",
				SimpleMode      = true,
				Templates = new PackagingTemplateViews
				{
					new()
					{
						Name   = "Testing",
						Fields = new List<TemplateField> { new TextField { FullName = "This works now right?", Id = 123 } }
					}
				},
				Gratitude = new BeneficiaryDefinitionView { Enabled = true, Amount = 1234 }
			};
			var content = sut.Serialize(instance);
			var target  = new NewDefinitionView();
			sut.UsingTarget(target).Deserialize(content).Should().BeEquivalentTo(instance);
		}

		public sealed class NewDefinitionView : DefinitionView
		{
			public Guid? Result { get; set; }
		}

		public class DefinitionView
		{
			public MarketClassification Market { get; set; }

			public string Name { get; set; } = default!;

			public string Description { get; set; } = string.Empty;

			public string DescriptionFull { get; set; }

			public string Identifier { get; set; } = default!;

			public decimal Price { get; set; }

			public decimal Average { get; set; }

			public PackagingTemplateViews Templates { get; set; } = default!;

			public bool SimpleMode { get; set; } = true;

			public BeneficiaryDefinitionView Gratitude { get; set; } = default!;
		}

		public sealed class BeneficiaryDefinitionView
		{
			public bool Enabled { get; set; }

			public float Amount { get; set; } = .1f;
		}

		public enum MarketClassification : byte
		{
			Standard    = 1,
			Featured    = 2,
			Treasure    = 3,
			Traditional = 4,
			Mature      = 5
		}

		public sealed class PackagingTemplateViews : SelectedCollection<PackagingTemplate>
		{
			public PackagingTemplateViews() : base(Array.Empty<PackagingTemplate>()) {}

			public PackagingTemplateViews(IEnumerable<PackagingTemplate> list) : base(list) {}

			public ICollection<TemplateFieldValue> Values { get; set; } = default!;
		}

		public class SelectedCollection<T> : List<T>
		{
			public SelectedCollection(IEnumerable<T> list) : base(list) {}

			public virtual T Selected { get; set; }
		}

		public class PackagingTemplate : Template {}

		public abstract class Template
		{
			public int Id { get; set; }

			public DateTimeOffset Created { get; set; }

			public string Identifier { get; set; } = default!;

			public string Name { get; set; } = default!;

			public string Description { get; set; }

			public ICollection<TemplateField> Fields { get; set; } = default!;
		}

		public class TextField : TemplateField
		{
			public string FullName { get; set; } = default!;
		}

		public abstract class TemplateField
		{
			public int Id { get; set; }

			public DateTimeOffset Created { get; set; }

			public string Identifier { get; set; } = default!;

			public string Name { get; set; } = default!;

			public string Description { get; set; }

			public bool IsRequired { get; set; }
		}

		public class TemplateFieldValue
		{
			public long Id { get; set; }

			public TemplateField Field { get; set; } = default!;

			public TemplateValue Value { get; set; } = default!;

			public void Deconstruct(out TemplateField field, out TemplateValue value)
			{
				field = Field;
				value = Value;
			}
		}

		public abstract class TemplateValue
		{
			public long Id { get; set; }

			public bool Enabled { get; set; }

			public TemplateValue Clone() => MemberwiseClone().To<TemplateValue>();
		}
	}
}