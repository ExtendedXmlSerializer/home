using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Properties;
using FluentAssertions;
using System.Collections.Immutable;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ContentModel.Properties
{
	public class TypePartsFormatterTests
	{
		[Fact]
		public void Verify()
		{
			TypePartsFormatter.Default.Get(new TypeParts("int", "testing", dimensions: ImmutableArray.Create(3, 4, 2)))
			                  .Should()
			                  .Be("testing:int^3,4,2");

			TypePartsFormatter.Default.Get(new TypeParts("class", "testing",
			                                             () => ImmutableArray
				                                             .Create(new TypeParts("Other", "sys", dimensions: ImmutableArray.Create(1)),
				                                                     new TypeParts("Another", "native")),
			                                             ImmutableArray.Create(6, 5, 9)))
			                  .Should()
			                  .Be("testing:class[sys:Other^1,native:Another]^6,5,9");
		}

		[Fact]
		public void VerifyTypes()
		{
			typeof(TypePartsFormatterTests).MakeArrayType()
			                               .Should()
			                               .Be(typeof(TypePartsFormatterTests[]));
			typeof(TypePartsFormatterTests).MakeArrayType(2)
			                               .Should()
			                               .Be(typeof(TypePartsFormatterTests[,]));
		}
	}
}