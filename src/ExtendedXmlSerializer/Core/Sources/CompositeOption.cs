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

using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Core.Sources
{
	public class CompositeOption<TParameter, TResult> : Selector<TParameter, TResult>,
	                                                    IOption<TParameter, TResult>
	{
		readonly ISpecification<TParameter> _specification;

		public CompositeOption(params IOption<TParameter, TResult>[] options)
			: this(new AnySpecification<TParameter>(options), options) {}

		public CompositeOption(ISpecification<TParameter> specification, params IOption<TParameter, TResult>[] options)
			: base(options)
		{
			_specification = specification;
		}

		public bool IsSatisfiedBy(TParameter parameter) => _specification.IsSatisfiedBy(parameter);
	}
}