using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReflectionModel
{
	public class TypeDefinitionIdentityComparerTests
	{
		[Fact]
		public void Verify()
		{
			var expected = typeof(IDictionary<,>).GetTypeInfo();
			var actual = typeof(Dictionary<,>).GetInterfaces()
			                                  .First()
			                                  .GetTypeInfo();
			Assert.NotStrictEqual(expected, actual);
			Assert.NotSame(expected, actual);

			var sut = TypeDefinitionIdentityComparer.Default;

			Assert.Equal(expected, actual, sut);
			var info = typeof(IDictionary<object, object>).GetTypeInfo();

			Assert.NotStrictEqual(expected, info);
			Assert.Equal(expected, info, sut);

			Assert.False(typeof(IDictionary<,>).IsAssignableFrom(typeof(Dictionary<,>)));
			Assert.True(typeof(IDictionary<string, object>).IsAssignableFrom(typeof(Dictionary<string, object>)));
		}
	}
}