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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public class AlteredContentExtension : ISerializerExtension
	{
		[UsedImplicitly]
		public AlteredContentExtension() :
			this(new TableSource<TypeInfo, ContentAlteration>(ReflectionModel.Defaults.TypeComparer),
			     new TableSource<MemberInfo, ContentAlteration>(MemberComparer.Default)) {}

		public AlteredContentExtension(ITableSource<TypeInfo, ContentAlteration> types,
		                               ITableSource<MemberInfo, ContentAlteration> members)
		{
			Types = types;
			Members = members;
		}

		public ITableSource<TypeInfo, ContentAlteration> Types { get; }
		public ITableSource<MemberInfo, ContentAlteration> Members { get; }

		public IServiceRepository Get(IServiceRepository parameter) => parameter.RegisterInstance(Types)
		                                                                        .RegisterInstance(Members)
		                                                                        .Decorate<IContents, Contents>()
		                                                                        .Decorate<IMemberContents, MemberContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly ITableSource<TypeInfo, ContentAlteration> _registrations;
			readonly IContents _contents;

			public Contents(ITableSource<TypeInfo, ContentAlteration> registrations, IContents contents)
			{
				_registrations = registrations;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var serializer = _contents.Get(parameter);
				var result = _registrations.IsSatisfiedBy(parameter)
					             ? new Serializer(_registrations.Get(parameter), serializer)
					             : serializer;
				return result;
			}
		}

		sealed class MemberContents : IMemberContents
		{
			readonly ITableSource<MemberInfo, ContentAlteration> _registrations;
			readonly IMemberContents _contents;

			public MemberContents(ITableSource<MemberInfo, ContentAlteration> registrations, IMemberContents contents)
			{
				_registrations = registrations;
				_contents = contents;
			}

			public ISerializer Get(IMember parameter)
			{
				var serializer = _contents.Get(parameter);
				var result = _registrations.IsSatisfiedBy(parameter.Metadata)
					             ? new Serializer(_registrations.Get(parameter.Metadata), serializer)
					             : serializer;
				return result;
			}
		}


		sealed class Serializer : ISerializer
		{
			readonly IAlteration<object> _read;
			readonly IAlteration<object> _write;
			readonly ISerializer _serializer;

			public Serializer(ContentAlteration alteration, ISerializer serializer) : this(alteration.Read, alteration.Write,
			                                                                               serializer) {}

			public Serializer(IAlteration<object> read, IAlteration<object> write, ISerializer serializer)
			{
				_read = read;
				_write = write;
				_serializer = serializer;
			}

			public object Get(IFormatReader parameter) => _read.Get(_serializer.Get(parameter));

			public void Write(IFormatWriter writer, object instance)
			{
				_serializer.Write(writer, _write.Get(instance));
			}
		}
	}
}