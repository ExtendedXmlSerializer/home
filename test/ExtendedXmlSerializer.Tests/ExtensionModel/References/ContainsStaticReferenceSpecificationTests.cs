using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.References
{
	public class ContainsStaticReferenceSpecificationTests
	{
		[Fact]
		public void VerifyFixed()
		{
			using (var services = new ServicesSupport())
			{
				var sut = services.Get<ContainsStaticReferenceSpecification>();
				Assert.False(sut.IsSatisfiedBy(typeof(Fixed).GetTypeInfo()));
			}
		}

		[Fact]
		public void VerifyNonReferencingVariable()
		{
			using (var services = new ServicesSupport())
			{
				var sut = services.Get<ContainsStaticReferenceSpecification>();
				Assert.False(sut.IsSatisfiedBy(typeof(NonReferencingVariable).GetTypeInfo()));
			}
		}

		[Fact]
		public void VerifyReferencingVariable()
		{
			using (var services = new ServicesSupport())
			{
				var sut = services.Get<ContainsStaticReferenceSpecification>();
				Assert.True(sut.IsSatisfiedBy(typeof(ReferencingVariable).GetTypeInfo()));
			}
		}

		[Fact]
		public void VerifyReferencingVariableWithInterface()
		{
			using (var services = new ServicesSupport())
			{
				var sut = services.Get<ContainsStaticReferenceSpecification>();
				Assert.True(sut.IsSatisfiedBy(typeof(ReferencingVariableWithInterface).GetTypeInfo()));
			}
		}

		[Fact]
		public void VerifyIndirectReferencingVariable()
		{
			using (var services = new ServicesSupport())
			{
				var sut = services.Get<ContainsStaticReferenceSpecification>();
				Assert.True(sut.IsSatisfiedBy(typeof(IndirectReferencingVariable).GetTypeInfo()));
			}
		}

		class ReferencingVariableWithInterface : NonReferencingVariable
		{
			[UsedImplicitly]
			public IWriter Writer { get; set; }
		}

		class ReferencingVariable : NonReferencingVariable
		{
			[UsedImplicitly]
			public Concrete Concrete { get; set; }
		}

		class IndirectReferencingVariable : NonReferencingVariable
		{
			[UsedImplicitly]
			public UnrelatedWithRelatedProperty Unrelated { get; set; }
		}

		[UsedImplicitly]
		class UnrelatedWithRelatedProperty
		{
			[UsedImplicitly]
			public AbstractBase AbstractBase { get; set; }
		}

		sealed class Fixed
		{
			[UsedImplicitly]
			public string String { get; set; }

			[UsedImplicitly]
			public double Double { get; set; }

			[UsedImplicitly]
			public Lists Sealed { get; set; }
		}

		[UsedImplicitly]
		class Concrete : OtherAbstractBase {}

		class NonReferencingVariable
		{
			[UsedImplicitly]
			public AbstractBase Abstract { get; set; }

			[UsedImplicitly]
			public OtherAbstractBase OtherAbstractBase { get; set; }

			[UsedImplicitly]
			public Identity Identity { get; set; }
		}

		abstract class AbstractBase {}

		abstract class OtherAbstractBase {}
	}
}