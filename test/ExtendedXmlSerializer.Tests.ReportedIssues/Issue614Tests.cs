using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
    public sealed class Issue614Tests
    {
        [Fact]
        public void Verify()
        {
            var serializer = new ConfigurationContainer().Type<BaseNamespace.Container>()
                                                         .AddMigration(EmptyMigration.Default)
                                                         .Create()
                                                         .ForTesting();

            var container = new BaseNamespace.Container();
            container.Content.Add(new InheritNamespace.Inherit() { Check = new BaseNamespace.BaseCheck() });
            container.Content.Add(new BaseNamespace.Base() { Check = new InheritNamespace.InheritCheck() });
            serializer.Cycle(container).Should().BeEquivalentTo(container);
        }

        [Fact]
        public void VerifyWithoutMigration()
        {
            var serializer = new ConfigurationContainer().Type<BaseNamespace.Container>()
                                                         .Create()
                                                         .ForTesting();

            var container = new BaseNamespace.Container();
            container.Content.Add(new InheritNamespace.Inherit() { Check = new BaseNamespace.BaseCheck() });
            container.Content.Add(new BaseNamespace.Base() { Check = new InheritNamespace.InheritCheck() });
            serializer.Cycle(container).Should().BeEquivalentTo(container);
        }

    }
}

