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
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberedContents : IContents
	{
		readonly IInstanceMemberSerializations _instances;
		readonly IInnerContentServices         _services;
		readonly Action<IFormatReader> _missing;

		public MemberedContents(IInstanceMemberSerializations instances, IInnerContentServices services, Action<IFormatReader> missing)
		{
			_instances = instances;
			_services  = services;
			_missing = missing;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var members = _instances.Get(parameter);
			var reader =
				_services.Create(parameter, new MemberInnerContentHandler(_instances.Get(parameter), _services, _services, _missing));
			var result = new Serializer(reader, new MemberListWriter(members));
			return result;
		}
	}
}