// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using FluentAssertions;
using System.Reflection;
using Xunit;
using Identity = ExtendedXmlSerializer.ContentModel.Identification.Identity;

namespace ExtendedXmlSerializer.Tests.ContentModel.Reflection
{
	public class TypePartResolverTests
	{
		[Fact]
		public void Verify()
		{
			var sut = new TypePartResolver(Identities.Default);

			var @class = sut.Get(typeof(Class));
			@class.Dimensions.Should().BeNull();
			@class.GetArguments().Should().BeNull();

			var generic = sut.Get(typeof(Generic<int>));
			generic.Dimensions.Should().BeNull();
			generic.GetArguments().Should().HaveCount(1);

			var generic2 = sut.Get(typeof(Generic<int, bool>));
			generic2.Dimensions.Should().BeNull();
			generic2.GetArguments().Should().HaveCount(2);
		}

		[Fact]
		public void VerifyDimensions()
		{
			var sut = new TypePartResolver(Identities.Default);
			var one = sut.Get(typeof(Class[]));
			one.Dimensions.Should().BeEquivalentTo(1);
			one.GetArguments().Should().BeNull();

			sut.Get(typeof(Class[,])).Dimensions.Should().BeEquivalentTo(2);
			sut.Get(typeof(Class[,][][])).Dimensions.Should().BeEquivalentTo(2, 1, 1);
			sut.Get(typeof(Class[,][][,,,])).Dimensions.Should().BeEquivalentTo(2, 1, 4);

		}

		sealed class Class {}

		sealed class Generic<T> {}

		sealed class Generic<T1, T2> {}


		sealed class Identities : IIdentities
		{
			public static Identities Default { get; } = new Identities();

			Identities() {}

			public IIdentity Get(TypeInfo parameter) => new Identity(parameter.Name, "testing");
		}
	}
}