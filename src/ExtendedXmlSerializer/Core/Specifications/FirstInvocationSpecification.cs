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

using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core.Specifications
{
	sealed class FirstInvocationSpecification<T> : ISpecification<T>
	{
		readonly ConditionMonitor _monitor;

		public FirstInvocationSpecification() : this(new ConditionMonitor()) {}

		public FirstInvocationSpecification(ConditionMonitor monitor) => _monitor = monitor;

		public bool IsSatisfiedBy(T _) => _monitor.Apply();
	}

	sealed class FirstInvocationByParameterSpecification<T> : ISpecification<T> where T : class
	{
		readonly IParameterizedSource<T, ConditionMonitor> _conditions;

		public FirstInvocationByParameterSpecification() : this(new Conditions<T>()) {}

		public FirstInvocationByParameterSpecification(IParameterizedSource<T, ConditionMonitor> monitors)
			=> _conditions = monitors;

		public bool IsSatisfiedBy(T parameter) => _conditions.Get(parameter).Apply();
	}
}