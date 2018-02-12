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
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberedContents<T> : FixedInstanceSource<IContentSerializer<T>>, IContents<T>
	{
		public MemberedContents(IInnerContentReader<T> reader, IMemberedContentWriter<T> writer) : base(new ContentSerializer<T>(reader, writer)) {}
	}


	sealed class MemberedContents : IContents
	{
		readonly IMemberSerializations _serializations;
		readonly IInnerContentServices _services;

		public MemberedContents(IMemberSerializations serializations, IInnerContentServices services)
		{
			_serializations = serializations;
			_services = services;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var members = _serializations.Get(parameter);
			var reader = _services.Create(parameter, new MemberInnerContentHandler(members, _services, _services));
			var result = new Serializer(reader, new MemberListWriter(members));
			return result;
		}
	}
}