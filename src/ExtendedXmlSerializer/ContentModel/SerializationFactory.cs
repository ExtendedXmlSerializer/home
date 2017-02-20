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
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.ContentModel
{
	class SerializationFactory : ISerializationFactory
	{
		public static SerializationFactory Default { get; } = new SerializationFactory();
		SerializationFactory() : this(Configuration.Defaults.Property, Configuration.Defaults.Field) {}

		readonly ISpecification<PropertyInfo> _property;
		readonly ISpecification<FieldInfo> _field;

		public SerializationFactory(ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
		{
			_property = property;
			_field = field;
		}

		public ISerialization Get(IExtendedXmlConfiguration parameter)
		{
			var policy = parameter.Get();
			var factory = new Factory(new ContainerOptions(), _property.And(policy), _field.And(policy)).ToDelegate();
			var result = new Serialization(factory);
			return result;
		}

		class Factory : IParameterizedSource<ISerialization, ISerializationProfile>
		{
			readonly IContainerOptions _options;
			readonly ISpecification<PropertyInfo> _property;
			readonly ISpecification<FieldInfo> _field;

			public Factory(IContainerOptions options, ISpecification<PropertyInfo> property, ISpecification<FieldInfo> field)
			{
				_options = options;
				_property = property;
				_field = field;
			}

			public ISerializationProfile Get(ISerialization parameter)
				=> new SerializationProfile(
					parameter,
					_property,
					_field,
					MemberAliases.Default,
					MemberOrder.Default,
					_options.Get(parameter).AsReadOnly()
				);
		}
	}
}