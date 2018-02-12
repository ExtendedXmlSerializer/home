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

using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RuntimeSerializer : IRuntimeSerializer
	{
		readonly ISpecification<object> _specification;
		readonly IMemberSerializer _property;
		readonly IMemberSerializer _content;

		public RuntimeSerializer(ISpecification<object> specification, IMemberSerializer property, IMemberSerializer content)
		{
			_specification = specification;
			_property = property;
			_content = content;
		}

		public IMemberSerializer Get(object parameter) => _specification.IsSatisfiedBy(parameter) ? _property : _content;

		public object Get(IFormatReader parameter) => _content.Get(parameter);

		public void Write(IFormatWriter writer, object instance) => _content.Write(writer, instance);
		public IMember Profile => _content.Profile;

		public IMemberAccess Access => _content.Access;
	}

	interface IRuntimeContent<T> : IParameterizedSource<Writing<T>, IRuntimeContentWriter<T>> {}

	interface IRuntimeContentWriter<T> : ISpecification<object>, IMemberContentWriter<T> {}

	class RuntimeContentWriter<T> : MemberContentWriter<T>, IRuntimeContentWriter<T>
	{
		readonly ISpecification<object> _specification;

		public RuntimeContentWriter(IContentWriter<T> writer, IMember member)
			: this(NeverSpecification<object>.Default, writer, member) {}

		public RuntimeContentWriter(ISpecification<object> specification, IContentWriter<T> writer, IMember member)
			: base(writer, member) => _specification = specification;

		public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);
	}

	/*sealed class ConditionalRuntimeContent<T> : ConditionalInstanceSource<Writing<T>, MemberContent<T>>, IRuntimeContent<T>
	{
		public ConditionalRuntimeContent(ISpecification<Writing<T>> specification, MemberContent<T> @true, MemberContent<T> @false) : base(specification, @true, @false) {}
	}*/

	sealed class FixedRuntimeContent<T> : FixedInstanceSource<Writing<T>, IRuntimeContentWriter<T>>, IRuntimeContent<T>
	{
		public FixedRuntimeContent(IRuntimeContentWriter<T> instance) : base(instance) {}
	}
}