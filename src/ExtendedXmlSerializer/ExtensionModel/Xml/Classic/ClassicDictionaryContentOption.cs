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
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicDictionaryContentOption : ContentOptionBase
	{
		readonly IContentsServices _contents;
		readonly IDictionaryEnumerators _enumerators;
		readonly IDictionaryEntries _entries;

		public ClassicDictionaryContentOption(IActivatingTypeSpecification specification, IContentsServices contents,
		                                      IDictionaryEnumerators enumerators, IDictionaryEntries entries)
			: base(specification.And(IsDictionaryTypeSpecification.Default))
		{
			_contents = contents;
			_enumerators = enumerators;
			_entries = entries;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var entry = _entries.Get(parameter);
			var reader = _contents.Create(parameter, new ConditionalContentHandler(_contents, new CollectionContentHandler(entry, _contents)));
			var result = new Serializer(reader, new EnumerableWriter(_enumerators, entry));
			return result;
		}
	}
}