using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue445Tests
	{
		[Fact]
		public void Verify()
		{
			const string content = @"<Issue445Tests-SampleModel>
	<FirstName>default_first_name</FirstName>
	<LastName></LastName>
	<ListOfItems>
		<Capacity>4</Capacity>
		<SampleItem>
			<ItemName>item1</ItemName>
			<ItemValue>1</ItemValue>
		</SampleItem>
		<SampleItem>
			<ItemName>item2</ItemName>
			<ItemValue>2</ItemValue>
		</SampleItem>
	</ListOfItems>
</Issue445Tests-SampleModel>";

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(SampleModel))
			                                             .Create()
			                                             .ForTesting();

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(content);
			writer.Flush();
			stream.Position = 0;

			using var reader = XmlReader.Create(stream);


			var result = (SampleModel)serializer.Deserialize(reader);

			result.LastName.Should().Be(new SampleModel().LastName);
		}

		[Fact]
		public void VerifyReported()
		{
			const string content = @"<Issue445Tests-SampleModel>
	<FirstName>default_first_name</FirstName>
	<LastName></LastName>
	<ListOfItems>
		<Capacity>4</Capacity>
		<SampleItem>
			<ItemName>item1</ItemName>
			<ItemValue>1</ItemValue>
		</SampleItem>
		<SampleItem>
			<ItemName>item2</ItemName>
			<ItemValue>2</ItemValue>
		</SampleItem>
	</ListOfItems>
</Issue445Tests-SampleModel>";

			var serializer = new ConfigurationContainer().EnableImplicitTyping(typeof(SampleModel))
			                                             .Create()
			                                             .ForTesting();

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(content);
			writer.Flush();
			stream.Position = 0;

			using var reader = new CustomXmlTextReader(stream) { Normalization = false };


			var result = (SampleModel)serializer.Deserialize(reader);

			result.LastName.Should().Be(new SampleModel().LastName);

		}

		sealed class CustomXmlTextReader : XmlTextReader
		{
			public CustomXmlTextReader([NotNull] Stream input) : base(input) {}

			public override bool CanReadValueChunk => true;
		}

		public class SampleModel
		{
			private string firstName = "default_first_name";

			public string FirstName
			{
				get => firstName;
				set
				{
					if (!string.IsNullOrEmpty(value))
					{
						firstName = value;
					}
				}
			}

			private string lastName = "default_last_name";

			public string LastName
			{
				get => lastName;
				set
				{
					if (!string.IsNullOrEmpty(value))
					{
						lastName = value;
					}
				}
			}

			List<SampleItem> listOfItems;

			public List<SampleItem> ListOfItems
			{
				get
				{
					if (listOfItems == null)
					{
						listOfItems = new List<SampleItem>
						{
							new SampleItem
							{
								ItemName  = "item1",
								ItemValue = 1
							},
							new SampleItem
							{
								ItemName  = "item2",
								ItemValue = 2
							}
						};
					}

					return listOfItems;
				}

				set => listOfItems = value;
			}
		}

		public class SampleItem
		{
			public string ItemName { get; set; }
			public int ItemValue { get; set; }
		}
	}
}