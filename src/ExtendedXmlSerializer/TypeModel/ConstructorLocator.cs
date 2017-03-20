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

using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.TypeModel
{
	public sealed class ConstructorLocator : ReferenceCacheBase<TypeInfo, ConstructorInfo>, IConstructorLocator
	{
		public static ConstructorLocator Default { get; } = new ConstructorLocator();
		ConstructorLocator() : this(ValidConstructorSpecification.Default, Constructors.Default) {}

		readonly IValidConstructorSpecification _specification;
		readonly IConstructors _constructors;

		public ConstructorLocator(IValidConstructorSpecification specification, IConstructors constructors)
		{
			_specification = specification;
			_constructors = constructors;
		}

		protected override ConstructorInfo Create(TypeInfo parameter)
		{
			var constructors = _constructors.Get(parameter).ToImmutableArray();
			var length = constructors.Length;
			for (var i = 0; i < length; i++)
			{
				var constructor = constructors[i];
				if (_specification.IsSatisfiedBy(constructor))
				{
					return constructor;
				}
			}
			return null;
		}
	}
}