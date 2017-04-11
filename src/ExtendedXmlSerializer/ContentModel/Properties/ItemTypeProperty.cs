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
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class ItemTypeProperty : Property<TypeInfo>
	{
		public static ItemTypeProperty Default { get; } = new ItemTypeProperty();
		ItemTypeProperty() : this(new FrameworkIdentity("item")) {}

		public ItemTypeProperty(IIdentity identity)
			: base(new Reader(new TypedParsingReader(identity)), new TypedFormattingWriter(identity), identity) {}

		sealed class Reader : IReader<TypeInfo>
		{
			readonly IReader<TypeInfo> _reader;

			public Reader(IReader<TypeInfo> reader)
			{
				_reader = reader;
			}

			public TypeInfo Get(IFormatReader parameter) => _reader.Get(parameter)?.MakeArrayType().GetTypeInfo();
		}
	}
}