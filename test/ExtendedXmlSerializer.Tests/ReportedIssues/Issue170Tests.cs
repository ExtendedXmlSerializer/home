using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public class Issue170Tests
	{
		//Props
		public string Name { get; set; }

		public double[] complex1D { get; set; }
		public double[][] complex2D { get; set; }
		public double[][][] complex3D { get; set; }


		[Fact]
		public void Verify()
		{
			int _one = 1, two = 2, three = 3;
			complex1D = new double[_one];
			complex2D = new double[_one][];
			complex3D = new double[_one][][];

			for (int i = 0; i < _one; i++)
			{
				complex1D[i] = i;
				complex2D[i] = new double[two];
				complex3D[i] = new double[two][];

				for (int j = 0; j < two; j++)
				{
					complex2D[i][j] = j;
					complex3D[i][j] = new double[three];
					for (int k = 0; k < three; k++)
						complex3D[i][j][k] = k;
				}

			}

			var support = new ConfigurationContainer().ForTesting();
			support
				.Cycle(complex3D)
				.ShouldBeEquivalentTo(complex3D);

			support
				.Cycle(complex2D)
				.ShouldBeEquivalentTo(complex2D);
		}
	}
}
