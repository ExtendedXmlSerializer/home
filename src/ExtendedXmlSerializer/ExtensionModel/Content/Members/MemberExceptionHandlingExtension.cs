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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using System;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class MemberExceptionHandlingExtension : ISerializerExtension
	{
		public static MemberExceptionHandlingExtension Default { get; } = new MemberExceptionHandlingExtension();

		MemberExceptionHandlingExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberSerializers, MemberSerializers>()
			            .Decorate<IMemberHandler, MemberHandler>();

		public void Execute(IServices parameter) {}

		sealed class MemberHandler : IMemberHandler
		{
			readonly IMemberHandler _handler;

			public MemberHandler(IMemberHandler handler) => _handler = handler;

			public void Handle(IInnerContent contents, IMemberSerializer member)
			{
				try
				{
					_handler.Handle(contents, member);
				}
				catch (Exception e)
				{
					var line = contents.Get()
					                   .Get()
					                   .To<IXmlLineInfo>();
					throw new
						InvalidOperationException($"An exception was encountered while deserializing member '{member.Profile.Metadata.ReflectedType}.{member.Profile.Name}'.",
						                          new XmlException(e.Message, e, line.LineNumber, line.LinePosition));
				}
			}
		}

		sealed class MemberSerializers : IMemberSerializers
		{
			readonly IMemberSerializers _serializers;

			public MemberSerializers(IMemberSerializers serializers) => _serializers = serializers;

			public IMemberSerializer Get(IMember parameter)
			{
				var origin     = _serializers.Get(parameter);
				var serializer = new MemberSerializer(origin);
				var result = origin is PropertyMemberSerializer property
					             ? new PropertyMemberSerializer(property)
					             : origin is IRuntimeSerializer runtime
						             ? (IMemberSerializer)new RuntimeSerializer(runtime)
						             : serializer;
				return result;
			}
		}

		sealed class RuntimeSerializer : IRuntimeSerializer
		{
			readonly IRuntimeSerializer _serializer;

			public RuntimeSerializer(IRuntimeSerializer serializer) => _serializer = serializer;

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				_serializer.Write(writer, instance);
			}

			public IMember Profile => _serializer.Profile;

			public IMemberAccess Access => _serializer.Access;

			public IMemberSerializer Get(object parameter) => _serializer.Get(parameter);
		}

		sealed class MemberSerializer : IMemberSerializer
		{
			readonly IMemberSerializer _serializer;

			public MemberSerializer(IMemberSerializer serializer) => _serializer = serializer;

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				try
				{
					_serializer.Write(writer, instance);
				}
				catch (Exception e)
				{
					throw new
						InvalidOperationException($"An exception was encountered while serializing member '{Profile.Metadata.ReflectedType}.{Profile.Name}'.  Provided instance is '{instance}'.",
						                          e);
				}
			}

			public IMember Profile => _serializer.Profile;

			public IMemberAccess Access => _serializer.Access;
		}
	}
}