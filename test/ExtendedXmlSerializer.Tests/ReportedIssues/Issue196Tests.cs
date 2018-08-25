using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue196Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer().EnableParameterizedContent()
			                                             .Extend(WritableParameterizedContentExtension.Default)
			                                             .Create()
			                                             .ForTesting();

			var instance = new MutableSubject("Hello World!");
			var cycle = serializer.Cycle(instance);
			cycle.ShouldBeEquivalentTo(instance);
			cycle.Message.Should()
			     .NotBeNull();
		}

		sealed class MutableSubject
		{
			public MutableSubject(string message) => Message = message;

			public string Message { get; set; }
		}

		sealed class WritableParameterizedContentExtension : ISerializerExtension
		{
			public static WritableParameterizedContentExtension Default { get; } = new WritableParameterizedContentExtension();

			WritableParameterizedContentExtension() {}

			public IServiceRepository Get(IServiceRepository parameter)
				=> parameter.RegisterWithDependencies<IMemberSpecifications, MemberSpecifications>();

			public void Execute(IServices parameter) {}

			sealed class MemberSpecifications : Items<ISpecification<IMember>>, IMemberSpecifications
			{
				public MemberSpecifications(IsValidMemberType specification) : base(specification) {}
			}
		}
	}
}
