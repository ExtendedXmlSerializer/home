using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue173Tests
    {
	    [Fact]
	    public void Verify()
	    {
		    //int adultC = 1;
		    string childC = "COMPLEXCHILDRENDATA";

		    Child orgC = new Child
		    {
			    Name = "Tom",
			    Complex = childC,
		    };

		    Adult orgA = new Adult
		    {
			    Name = "Andy",
			    Complex = new[] { 1.0, 2.0 }
		    };

		    var container = new ConfigurationContainer().ForTesting();
			container.Cycle(orgC).ShouldBeEquivalentTo(orgC);
			container.Cycle(orgA).ShouldBeEquivalentTo(orgA);

	    }

	    public abstract class Person
	    {
		    public string Name { get; set; }
		    public abstract object Complex { get; set; }

	    }

	    public class Child : Person
	    {
		    string _complex;

		    public override object Complex
		    {
			    get { return _complex; }
			    set { _complex = (string)value; }
		    }
	    }

	    public class Adult : Person
	    {
		    //int IDs;
		    double[] IDs;
		    public override object Complex
		    {
			    get { return IDs; }
			    //set { IDs = (int)value; }
			    set { IDs = (double[])value; }

		    }
	    }

    }
}
