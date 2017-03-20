// MIT License
//
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.Support;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel
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