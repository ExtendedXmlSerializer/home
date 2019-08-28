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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class SchemaTypeExtension : ISerializerExtension
	{
		public static SchemaTypeExtension Default { get; } = new SchemaTypeExtension();

		SchemaTypeExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IClassification, Classification>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Classification : IClassification
		{
			readonly IClassification   _classification;
			readonly IReader<TypeInfo> _reader;

			public Classification(IClassification classification) : this(classification, Reader.Instance) {}

			public Classification(IClassification classification, IReader<TypeInfo> reader)
			{
				_classification = classification;
				_reader         = reader;
			}

			public TypeInfo Get(IFormatReader parameter)
			{
				var isSatisfiedBy = parameter.IsSatisfiedBy(SchemaType.Instance);
				return isSatisfiedBy
					       ? _reader.Get(parameter)
					       : _classification.Get(parameter);
			}
		}

		sealed class SchemaType : ContentModel.Identification.Identity
		{
			public static SchemaType Instance { get; } = new SchemaType();

			SchemaType() : base("type", "http://www.w3.org/2001/XMLSchema-instance") {}
		}

		sealed class Reader : TypedParsingReader
		{
			public static Reader Instance { get; } = new Reader();

			Reader() : base(SchemaType.Instance) {}
		}
	}
}