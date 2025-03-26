using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue645Tests
    {
        
#if CORE
	    [Fact]
	    public void VerifyTupleContainer()
	    {
		    var sut      = new ConfigurationContainer().EnableParameterizedContent().Create().ForTesting();
		    var instance = new TupleContainer { Subject = Tuple.Create(123) };
		    sut.Cycle(instance).Should().BeEquivalentTo(instance);
	    }

	    public class TupleContainer
	    {
		    public ITuple Subject { get; set; }
	    }
#endif
	    [Fact]
	    public void VerifyTuple()
	    {
		    var sut      = new ConfigurationContainer().EnableParameterizedContent().Create().ForTesting();
		    var instance = Tuple.Create(123);
		    sut.Cycle(instance).Should().BeEquivalentTo(instance);
	    }
    }
}
