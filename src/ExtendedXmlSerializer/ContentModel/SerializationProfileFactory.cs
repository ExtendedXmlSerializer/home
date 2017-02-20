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

using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel
{
	class SerializationProfileFactory : IParameterizedSource<ISerialization, ISerializationProfile>
	{
		readonly IContainerOptions _options;
		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;
		readonly IRuntimeMemberSpecifications _specifications;
		readonly Members.IAliases _aliases;
		readonly IMemberConverters _converters;

		public SerializationProfileFactory(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field,
		                                   IRuntimeMemberSpecifications specifications, Members.IAliases aliases,
		                                   IMemberConverters converters, IContainerOptions options)
		{
			_options = options;
			_property = property;
			_field = field;
			_specifications = specifications;
			_aliases = aliases;
			_converters = converters;
		}

		public ISerializationProfile Get(ISerialization parameter)
			=> new SerializationProfile(_property, _field,
			                            parameter,
			                            new MemberSerializers(_specifications, _converters),
			                            _aliases,
			                            MemberOrder.Default,
			                            _options.Get(parameter).AsReadOnly()
			);
	}
}