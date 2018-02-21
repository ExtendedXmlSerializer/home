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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel
{
	public interface IContentWriter<T> : ICommand<Writing<T>> {}

	sealed class AssignedContentAwareWriter<T> : ConditionalInstanceContentWriter<T>
	{
		public AssignedContentAwareWriter(IContentWriter<T> @true, IContentWriter<T> @false) :
			this(AssignedSpecification<T>.Default, @true, @false) {}

		public AssignedContentAwareWriter(ISpecification<T> specification, IContentWriter<T> @true,
		                                  IContentWriter<T> @false) : base(specification, @true, @false) {}
	}

	class ConditionalInstanceContentWriter<T> : IContentWriter<T>
	{
		readonly ISpecification<T> _specification;
		readonly IContentWriter<T> _true;
		readonly IContentWriter<T> _false;

		public ConditionalInstanceContentWriter(ISpecification<T> specification, IContentWriter<T> @true,
		                                        IContentWriter<T> @false)
		{
			_specification = specification;
			_true          = @true;
			_false         = @false;
		}

		public void Execute(Writing<T> parameter)
		{
			var writer = _specification.IsSatisfiedBy(parameter.Instance) ? _true : _false;
			writer.Execute(parameter);
		}
	}

	class ConditionalContentWriter<T> : IContentWriter<T>
	{
		readonly ISpecification<Writing<T>> _specification;
		readonly IContentWriter<T>          _true;
		readonly IContentWriter<T>          _false;

		public ConditionalContentWriter(ISpecification<Writing<T>> specification, IContentWriter<T> @true,
		                                IContentWriter<T> @false)
		{
			_specification = specification;
			_true          = @true;
			_false         = @false;
		}

		public void Execute(Writing<T> parameter)
		{
			var writer = _specification.IsSatisfiedBy(parameter) ? _true : _false;
			writer.Execute(parameter);
		}
	}

	sealed class AssignedContentAwareReader<T> : ConditionalContentReader<T>
	{
		public AssignedContentAwareReader(IContentReader<T> @true, IContentReader<T> @false) :
			base(ContainsNullContentSpecification.Default, @true, @false) {}
	}

	class ConditionalContentReader<T> : IContentReader<T>
	{
		readonly ISpecification<IFormatReader> _specification;
		readonly IContentReader<T>             _true;
		readonly IContentReader<T>             _false;

		public ConditionalContentReader(ISpecification<IFormatReader> specification, IContentReader<T> @true,
		                                IContentReader<T> @false)
		{
			_specification = specification;
			_true          = @true;
			_false         = @false;
		}

		public T Get(IFormatReader parameter)
		{
			var item   = _specification.IsSatisfiedBy(parameter) ? _true : _false;
			var result = item.Get(parameter);
			return result;
		}
	}

	sealed class ContainsNullContentSpecification : ContainsIdentitySpecification
	{
		public static ContainsNullContentSpecification Default { get; } = new ContainsNullContentSpecification();
		ContainsNullContentSpecification() : base(NullValueIdentity.Default) {}
	}

	class ContainsIdentitySpecification : ISpecification<IFormatReader>
	{
		readonly IIdentity _identity;
		public ContainsIdentitySpecification(IIdentity identity) => _identity = identity;

		public bool IsSatisfiedBy(IFormatReader parameter) => parameter.IsSatisfiedBy(_identity);
	}

	public interface INullContentWriter<T> : IContentWriter<T> {}

	public interface INullContentReader<out T> : IContentReader<T> {}

	sealed class NullContentWriter<T> : INullContentWriter<T>
	{
		public static NullContentWriter<T> Default { get; } = new NullContentWriter<T>();
		NullContentWriter() {}

		public void Execute(Writing<T> parameter) => parameter.Writer.Content(null);
	}

	sealed class NullContentReader<T> : FixedInstanceSource<IFormatReader, T>, INullContentReader<T>
	{
		public static NullContentReader<T> Default { get; } = new NullContentReader<T>();
		NullContentReader() : base(default(T)) {}
	}
}