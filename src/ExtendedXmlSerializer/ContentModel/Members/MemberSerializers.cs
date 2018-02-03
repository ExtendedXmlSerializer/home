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
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializers : IMemberSerializers
	{
		readonly IMemberAccessors _accessors;
		readonly IAttributeSpecifications _runtime;
		readonly IMemberConverters _converters;
		readonly IMemberContents _content;

		public MemberSerializers(IAttributeSpecifications runtime, IMemberAccessors accessors,
		                         IMemberConverters converters, IMemberContents content)
		{
			_runtime = runtime;
			_accessors = accessors;
			_converters = converters;
			_content = content;
		}

		public IMemberSerializer Get(IMember parameter)
		{
			var converter = _converters.Get(parameter);
			var access = _accessors.Get(parameter);
			var alteration = new DelegatedAlteration<object>(access.Get);
			var result = converter != null
				             ? Property(alteration, converter, parameter, access)
				             : Content(alteration, parameter, access);
			return result;
		}

		IMemberSerializer Property(IAlteration<object> alteration, IConverter converter, IMember profile,
		                           IMemberAccess access)
		{
			var serializer = new ConverterProperty<object>(converter, profile).Adapt();
			var member = new MemberSerializer(profile, access, serializer, Wrap(alteration, access, serializer));
			var runtime = _runtime.Get(profile.Metadata);
			IMemberSerializer property = new PropertyMemberSerializer(member);
			return runtime != null
				       ? new RuntimeSerializer(new AlteredSpecification<object>(alteration, runtime),
				                               property, Content(alteration, profile, access))
				       : property;
		}

		IMemberSerializer Content(IAlteration<object> alteration, IMember profile, IMemberAccess access)
		{
			var body = _content.Get(profile);
			var start = new Identity<object>(profile).Adapt();
			var writer = Wrap(alteration, access, new Enclosure(start, body));
			var result = new MemberSerializer(profile, access, body, writer);
			return result;
		}

		static IWriter Wrap(IAlteration<object> alteration, IMemberAccess access, IWriter writer)
			=> new AlteringWriter(alteration, new ConditionalWriter(access, writer));
	}
}