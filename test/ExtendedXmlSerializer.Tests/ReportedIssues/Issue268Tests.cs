using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Data;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue268Tests
	{
		[Fact]
		void Verify()
		{
			using (var instance = Create())
			{
				var table = new ConfigurationContainer().Type<DataTable>()
				                                        .UseClassicSerialization()
				                                        .Create()
				                                        .Cycle(instance);
				table.TableName.Should()
				     .Be(instance.TableName);

				table.Rows.Count.Should()
				     .Be(instance.Rows.Count);

				for (var i = 0; i < table.Rows.Count; i++)
				{
					table.Rows[i]["Name"]
					     .Should()
					     .Be(instance.Rows[i]["Name"]);
					table.Rows[i]["Marks"]
					     .Should()
					     .Be(instance.Rows[i]["Marks"]);
				}
			}
		}

		[Fact]
		void VerifyContainerProperty()
		{
			using (var instance = Create())
			{
				var table = new ConfigurationContainer().Type<DataTable>()
				                                        .UseClassicSerialization()
				                                        .Create()
				                                        .Cycle(new Container {Table = instance})
				                                        .Table;
				table.TableName.Should()
				     .Be(instance.TableName);

				table.Rows.Count.Should()
				     .Be(instance.Rows.Count);

				for (var i = 0; i < table.Rows.Count; i++)
				{
					table.Rows[i]["Name"]
					     .Should()
					     .Be(instance.Rows[i]["Name"]);
					table.Rows[i]["Marks"]
					     .Should()
					     .Be(instance.Rows[i]["Marks"]);
				}
			}
		}

		sealed class Container
		{
			public DataTable Table { get; set; }
		}

		static DataTable Create()
		{
			var result = new DataTable {TableName = "Yo"};
			result.Columns.Add("Name");
			result.Columns.Add("Marks");
			Row(result, "First");
			Row(result, "Second");
			return result;
		}

		static void Row(DataTable table, string name)
		{
			var result = table.NewRow();
			result["Name"]  = name;
			result["Marks"] = "500";
			table.Rows.Add(result);
		}
	}
}