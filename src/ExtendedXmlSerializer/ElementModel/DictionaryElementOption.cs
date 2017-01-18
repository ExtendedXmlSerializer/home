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
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
    class DictionaryElementOption : CollectionElementOptionBase
    {
        private readonly IElements _elements;
        private readonly IActivatedElement _entry;
        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryElementOption(IElements elements, IElementNames names, IElementMembers members)
            : this(elements, names, members, DictionaryPairTypesLocator.Default) {}

        public DictionaryElementOption(IElements elements, IElementNames names, IElementMembers members,
                                       IDictionaryPairTypesLocator locator)
            : this(
                elements, names, members,
                new DictionaryEntryElement(members.Get(DictionaryEntryElement.DictionaryEntryType)), locator) {}

        public DictionaryElementOption(IElements elements, IElementNames names, IElementMembers members,
                                       IActivatedElement entry, IDictionaryPairTypesLocator locator)
            : base(IsDictionaryTypeSpecification.Default, elements, names, members)
        {
            _elements = elements;
            _entry = entry;
            _locator = locator;
        }

        protected override IElement Create(string name, TypeInfo collectionType, IMembers members,
                                           ICollectionItem elementType)
        {
            var pair = _locator.Get(collectionType);
            var item = new DictionaryItem(_entry, _elements.Build(pair.KeyType), _elements.Build(pair.ValueType));
            var result = new DictionaryElement(name, collectionType, members, item);
            return result;
        }
    }
}