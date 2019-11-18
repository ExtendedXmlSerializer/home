using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;
// ReSharper disable All

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

			for (var i = 0; i < _one; i++)
			{
				complex1D[i] = i;
				complex2D[i] = new double[two];
				complex3D[i] = new double[two][];

				for (var j = 0; j < two; j++)
				{
					complex2D[i][j] = j;
					complex3D[i][j] = new double[three];
					for (var k = 0; k < three; k++)
					{
						complex3D[i][j][k] = k;
					}
				}
			}

			var support = new ConfigurationContainer().ForTesting();
			support
				.Cycle(complex3D)
				.Should().BeEquivalentTo(complex3D);

			support
				.Cycle(complex2D)
				.Should().BeEquivalentTo(complex2D);
		}

		[Fact]
		public void VerifyMultidimensional2D()
		{
			var subject = new[,]
			{
				{1, 2, 3, 4},
				{5, 6, 7, 8}
			};

			var support = new ConfigurationContainer().ForTesting();
			support
				.Cycle(subject)
				.Should().BeEquivalentTo(subject);
		}

		[Fact]
		public void VerifyMultidimensional3D()
		{
			var subject = new[,,]
			{
				{
					{1, 2, 3, 4, 5},
					{6, 7, 8, 9, 10},
					{11, 12, 13, 14, 15}
				},
				{
					{1, 2, 3, 4, 5},
					{6, 7, 8, 9, 10},
					{11, 12, 13, 14, 15},
				}
			};

			var support = new ConfigurationContainer().ForTesting();
			support.Cycle(subject)
			       .Should().BeEquivalentTo(subject);
		}

		[Fact]
		public void VerifyMapDoesNotEmitForOneDimensionalArrays()
		{
			var subject = new[] {2, 3, 45, 32, 254};
			var support = new ConfigurationContainer().ForTesting();
			support.Assert(subject,
			               @"<?xml version=""1.0"" encoding=""utf-8""?><Array xmlns:exs=""https://extendedxmlserializer.github.io/v2"" exs:item=""int"" xmlns=""https://extendedxmlserializer.github.io/system""><int>2</int><int>3</int><int>45</int><int>32</int><int>254</int></Array>");
		}

		[Fact]
		public void VerifyMultidimensional2()
		{
			var instance = new Person_Multi() {Name = "Testing"};
			instance.Initialize();
			var support = new ConfigurationContainer().ForTesting();
			support
				.Cycle(instance)
				.Should().BeEquivalentTo(instance);
		}

		public class Person_Multi
		{
			int one = 1, two = 2, three = 3;

			//Props
			public string Name { [UsedImplicitly] get; set; }

			public double[] complex1D { get; set; }
			public double[,] complex2D { get; set; }
			public double[,,] complex3D { get; set; }

			//Method 2 initialize complexArr.
			public void Initialize()
			{
				complex1D = new double[one];
				complex2D = new double[one, two];
				complex3D = new double[one, two, three];

				for (var i = 0; i < one; i++)
				{
					complex1D[i] = i;

					for (var j = 0; j < two; j++)
					{
						complex2D[i, j] = j;
						for (var k = 0; k < three; k++)
						{
							complex3D[i, j, k] = k;
						}
					}
				}
			}
		}
	}
}