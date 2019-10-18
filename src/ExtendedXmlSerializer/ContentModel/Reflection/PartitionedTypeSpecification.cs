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

using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class PartitionedTypeSpecification : DecoratedSpecification<TypeInfo>,
	                                            IPartitionedTypeSpecification
	{
		public static PartitionedTypeSpecification Default { get; } = new PartitionedTypeSpecification();
		PartitionedTypeSpecification() : base(NotHappy.Default) {}
	}

	sealed class NotHappy : ISpecification<TypeInfo>
		// HACK: This is a bit of a hack -- ok a total hack, to pass a test for .NET Framework under special conditions: https://github.com/WojciechNagorski/ExtendedXmlSerializer/issues/248
		// This doesn't occur for .NET Core, but we will have to do better for v3 in any case.
	{
		public static NotHappy Default { get; } = new NotHappy();

		NotHappy() : this(typeof(object).Assembly) {}

		readonly Assembly _assembly;

		public NotHappy(Assembly assembly) => _assembly = assembly;

		public bool IsSatisfiedBy(TypeInfo parameter)
			=> parameter.Assembly != _assembly || !parameter.Namespace.Contains("System.Runtime.Remoting");
	}
}