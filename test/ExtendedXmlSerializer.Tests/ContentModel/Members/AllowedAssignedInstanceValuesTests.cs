using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Members
{
	public class AllowedAssignedInstanceValuesTests
	{
		[Fact]
		public void EmitValuesBasedOnInstanceDefaults()
		{
			var instance = new SubjectWithDefaultValue();
			new SerializationSupport().Assert(instance,
			                                  @"<?xml version=""1.0"" encoding=""utf-8""?><AllowedAssignedInstanceValuesTests-SubjectWithDefaultValue xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Members;assembly=ExtendedXmlSerializer.Tests""><SomeValue>This is a Default Value!</SomeValue></AllowedAssignedInstanceValuesTests-SubjectWithDefaultValue>");

			var configuration = new ConfigurationContainer().Emit(EmitBehaviors.Assigned);

			var support = new SerializationSupport(configuration);
			support.Assert(instance,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><AllowedAssignedInstanceValuesTests-SubjectWithDefaultValue xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ContentModel.Members;assembly=ExtendedXmlSerializer.Tests"" />");
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		class SubjectWithDefaultValue
		{
			public SubjectWithDefaultValue() : this("This is a Default Value!") {}

			public SubjectWithDefaultValue(string someValue) => SomeValue = someValue;

			public string SomeValue { get; set; }
		}
	}
}