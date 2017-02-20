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

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MemberSerializers : IMemberSerializers
	{
		readonly IRuntimeMemberSpecifications _specifications;
		readonly IMemberConverters _converters;

		public MemberSerializers(IRuntimeMemberSpecifications specifications, IMemberConverters converters)
		{
			_specifications = specifications;
			_converters = converters;
		}

		public IWriter Create(string name, IMetadata member, IWriter content)
		{
			var converter = _converters.Get(member.Metadata) ?? _converters.Get(member.MemberType);
			if (converter != null)
			{
				IWriter property = new MemberProperty(converter, name);
				var specification = _specifications.Get(member.Metadata);
				var writer = specification != null ? new RuntimeMember(specification, property, Content(name, content)) : property;
				return writer;
			}

			var result = Content(name, content);
			return result;
		}

		static Enclosure Content(string name, IWriter content) => new Enclosure(new MemberElement(name), content);
	}
}