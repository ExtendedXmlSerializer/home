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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AlteredMemberContents : IMemberContents
	{
		readonly static AssignedSpecification<IConverter> AssignedSpecification = AssignedSpecification<IConverter>.Default;

		readonly ISpecification<MemberInfo> _specification;
		readonly ISpecification<IConverter> _assigned;
		readonly IAlteration<IConverter> _alteration;
		readonly IMemberContents _contents;
		readonly IConverters _converters;
		readonly ISerializer _content;

		public AlteredMemberContents(ISpecification<MemberInfo> specification, IAlteration<IConverter> alteration,
		                               IMemberContents contents, IConverters converters, ISerializer content)
			: this(specification, AssignedSpecification, alteration, contents, converters, content) {}

		public AlteredMemberContents(ISpecification<MemberInfo> specification, ISpecification<IConverter> assigned,
		                               IAlteration<IConverter> alteration,
		                               IMemberContents contents, IConverters converters, ISerializer content)
		{
			_specification = specification;
			_assigned = assigned;
			_alteration = alteration;
			_contents = contents;
			_converters = converters;
			_content = content;
		}

		public ISerializer Get(IMember parameter)
		{
			var converter = _converters.Get(parameter.MemberType);
			var result = _assigned.IsSatisfiedBy(converter) && _specification.IsSatisfiedBy(parameter.Metadata)
							 ? new Serializer(_content, _alteration.Get(converter))
				             : _contents.Get(parameter);
			return result;
		}

		sealed class Serializer : ISerializer
		{
			readonly ISerializer _serializer;
			readonly IConverter _converter;
			public Serializer(ISerializer serializer, IConverter converter)
			{
				_serializer = serializer;
				_converter = converter;
			}

			public object Get(IFormatReader parameter) => _converter.Parse(_serializer.Get(parameter).AsValid<string>());

			public void Write(IFormatWriter writer, object instance) => _serializer.Write(writer, _converter.Format(instance));
		}
	}
}