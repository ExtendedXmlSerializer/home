using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
    public sealed class Issue612Tests
    {
        [Fact]
        public void Verify()
        {
            var serializer = new ConfigurationContainer().Type<BaseNamespace.Container>()
                                                         .AddMigration(EmptyMigration.Default)
                                                         .Create()
                                                         .ForTesting();

            var container = new BaseNamespace.Container();
            container.Content.Add(new InheritNamespace.Inherit() { Check = new InheritNamespace.InheritCheck() });
            serializer.Cycle(container).Should().BeEquivalentTo(container);
        }

        [Fact]
        public void VerifyWithoutMigration()
        {
            var serializer = new ConfigurationContainer().Type<BaseNamespace.Container>()
                                                         .Create()
                                                         .ForTesting();

            var container = new BaseNamespace.Container();
            container.Content.Add(new InheritNamespace.Inherit() { Check = new InheritNamespace.InheritCheck() });
            serializer.Cycle(container).Should().BeEquivalentTo(container);
        }

        [Fact]
        public void VerifyComplex()
        {
            var serializer = new ConfigurationContainer().Type<BaseNamespace.Container>()
                                                         .AddMigration(EmptyMigration.Default)
                                                         .Create()
                                                         .ForTesting();

            var container = new BaseNamespace.Container();
            container.Content.Add(new BaseNamespace.Base() { Check = new BaseNamespace.BaseCheck() });
            container.Content.Add(new InheritNamespace.Inherit() { Check = new InheritNamespace.InheritCheck() });
            container.Content.Add(new InheritNamespace.Inherit() { Check = new BaseNamespace.BaseCheck() });
            serializer.Cycle(container).Should().BeEquivalentTo(container);
        }

        [Fact]
        public void VerifyComplexWithoutMigration()
        {
            var serializer = new ConfigurationContainer().Type<BaseNamespace.Container>()
                                                         .Create()
                                                         .ForTesting();

            var container = new BaseNamespace.Container();
            container.Content.Add(new BaseNamespace.Base() { Check = new BaseNamespace.BaseCheck() });
            container.Content.Add(new InheritNamespace.Inherit() { Check = new InheritNamespace.InheritCheck() });
            container.Content.Add(new InheritNamespace.Inherit() { Check = new BaseNamespace.BaseCheck() });
            serializer.Cycle(container).Should().BeEquivalentTo(container);
        }

    }
    sealed class EmptyMigration : IEnumerable<Action<XElement>>
    {
        public static EmptyMigration Default { get; } = new EmptyMigration();

        EmptyMigration() { }

        public IEnumerator<Action<XElement>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

namespace BaseNamespace
{

    public class Container
    {
        public List<Base> Content { get; set; } = new List<Base>();
    }

    public class Base
    {
        public BaseCheck Check { get; set; }
    }
    public class BaseCheck { }
}

namespace InheritNamespace
{

    public class Inherit : BaseNamespace.Base { }


    public class InheritCheck : BaseNamespace.BaseCheck { }

}

