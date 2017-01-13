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

using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class ConvertMembers : WeakCacheBase<IConverter, IMembers>
    {
        private readonly ISerializationToolsFactory _tools;

        public ConvertMembers(ISerializationToolsFactory tools)
        {
            _tools = tools;
        }

        protected override IMembers Create(IConverter parameter)
        {
            /*var getter = new LegacyGetterFactory(_tools);
            var factories = new ReadOnlyCollectionMemberFactory(parameter, new EnumeratingReader(parameter), getter,
                                                                AddDelegates.Default);
            var factory =
                new CompositeMemberFactory(
                    new LegacyAssignableMemberElementFactory(parameter, new LegacyMemberTypeEmittingWriter(parameter),
                                                      getter), factories);
            var memberFactory = new LegacyMemberFactory(_tools, factory);
            var result = new Members.Members(memberFactory);
            return result;*/
            return null;
        }
    }
}