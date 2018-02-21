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
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel
{
	public interface IContentWriter<T> : ICommand<Writing<T>> {}

	class ConditionalContentWriter<T> : IContentWriter<T>
	{
		readonly ISpecification<Writing<T>> _specification;
		readonly IContentWriter<T>          _true;
		readonly IContentWriter<T>          _false;

		public ConditionalContentWriter(ISpecification<Writing<T>> specification, IContentWriter<T> @true)
			: this(specification, @true, EmptyContentWriter<T>.Default) {}

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

	sealed class ContainsUnassignedContentSpecification : ContainsIdentitySpecification
	{
		public static ContainsUnassignedContentSpecification Default { get; } = new ContainsUnassignedContentSpecification();
		ContainsUnassignedContentSpecification() : base(NullValueIdentity.Default) {}
	}

	class ContainsIdentitySpecification : ISpecification<IFormatReader>
	{
		readonly IIdentity _identity;
		public ContainsIdentitySpecification(IIdentity identity) => _identity = identity;

		public bool IsSatisfiedBy(IFormatReader parameter) => parameter.IsSatisfiedBy(_identity);
	}

	public interface IUnassignedContentWriter<T> : IContentWriter<T> {}

	public interface IUnassignedContentReader<out T> : IContentReader<T> {}

	sealed class UnassignedContentWriter<T> : IUnassignedContentWriter<T>
	{
		[UsedImplicitly]
		public static UnassignedContentWriter<T> Default { get; } = new UnassignedContentWriter<T>();
		UnassignedContentWriter() {}

		public void Execute(Writing<T> parameter) => parameter.Writer.Content(null);
	}

	sealed class UnassignedContentReader<T> : FixedInstanceSource<IFormatReader, T>, IUnassignedContentReader<T>
	{
		[UsedImplicitly]
		public static UnassignedContentReader<T> Default { get; } = new UnassignedContentReader<T>();
		UnassignedContentReader() : base(default(T)) {}
	}
}