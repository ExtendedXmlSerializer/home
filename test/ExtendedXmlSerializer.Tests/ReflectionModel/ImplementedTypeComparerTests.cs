using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReflectionModel
{
	public class ImplementedTypeComparerTests
	{
		[Fact]
		public void Verify()
		{
			var expected = typeof(IDictionary<,>).GetTypeInfo();
			var actual = typeof(Dictionary<,>).GetInterfaces()
			                                  .First()
			                                  .GetTypeInfo();

			var sut = ImplementedTypeComparer.Default;

			Assert.Equal(expected, actual, sut);
		}

		[Fact]
		public void Inherited()
		{
			var expected = typeof(IDictionary<,>).GetTypeInfo();
			var sut      = ImplementedTypeComparer.Default;

			Assert.Equal(expected, typeof(Dictionary).GetTypeInfo(), sut);
		}

		class Dictionary : Dictionary<string, string> {}
	}
}