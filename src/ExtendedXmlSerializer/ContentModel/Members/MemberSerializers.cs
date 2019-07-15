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
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializers : IMemberSerializers
	{
		readonly IMemberAccessors         _accessors;
		readonly IAttributeSpecifications _runtime;
		readonly IMemberConverters        _converters;
		readonly IMemberContents          _content;

		public MemberSerializers(IAttributeSpecifications runtime, IMemberAccessors accessors,
		                         IMemberConverters converters, IMemberContents content)
		{
			_runtime    = runtime;
			_accessors  = accessors;
			_converters = converters;
			_content    = content;
		}

		public IMemberSerializer Get(IMember parameter)
		{
			var converter  = _converters.Get(parameter);
			var access     = _accessors.Get(parameter);
			var alteration = new DelegatedAlteration<object>(access.Get);
			var result = converter != null
				             ? Property(alteration, converter, parameter, access)
				             : Content(parameter, access);
			return result;
		}

		IMemberSerializer Property(IAlteration<object> alteration, IConverter converter, IMember profile,
		                           IMemberAccess access)
		{
			var serializer = new ConverterProperty<object>(converter, profile).Adapt();
			var member     = new MemberSerializer(profile, access, serializer, new Writer(access, serializer));
			var runtime    = _runtime.Get(profile.Metadata);
			var property   = (IMemberSerializer)new PropertyMemberSerializer(member);
			return runtime != null
				       ? new RuntimeSerializer(new AlteredSpecification<object>(alteration, runtime),
				                               property, Content(profile, access))
				       : property;
		}

		IMemberSerializer Content(IMember profile, IMemberAccess access)
		{
			var body     = _content.Get(profile);
			var identity = new Identity<object>(profile);
			var composite = CollectionItemTypeLocator.Default.Get(profile.MemberType)
			                                         ?.Name == profile.Name
				                ? (IWriter<object>)new MemberPropertyWriter(identity)
				                : identity;
			var start  = composite.Adapt();
			var writer = new Writer(access, new Enclosure(start, body));
			var result = new MemberSerializer(profile, access, body, writer);
			return result;
		}

		sealed class Writer : IWriter
		{
			readonly IMemberAccess   _access;
			readonly IWriter<object> _writer;

			public Writer(IMemberAccess access, IWriter<object> writer)
			{
				_access = access;
				_writer = writer;
			}

			public void Write(IFormatWriter writer, object instance)
			{
				if (_access.Instance.IsSatisfiedBy(instance))
				{
					var member = _access.Get(instance);
					if (_access.IsSatisfiedBy(member))
					{
						_writer.Write(writer, member);
					}
				}
			}
		}
	}
}