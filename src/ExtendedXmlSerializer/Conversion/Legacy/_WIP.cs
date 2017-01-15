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

using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class LegacyElements : Elements
    {
        public static new LegacyElements Default { get; } = new LegacyElements();
        LegacyElements() : this(LegacyElementMembers.Default) {}

        public LegacyElements(IElementMembers members) : base(LegacyElementNames.Default, members) {}
    }

    sealed class LegacyElementsTooling : WeakCacheBase<ISerializationToolsFactory, IElementSelector>
    {
        public static LegacyElementsTooling Default { get; } = new LegacyElementsTooling();
        LegacyElementsTooling() {}

        protected override IElementSelector Create(ISerializationToolsFactory parameter)
            => new LegacyElements(new LegacyElementMembers(new LegacyMemberElementSelector(parameter)));
    }

    class LegacyElementMembers : ElementMembers
    {
        public static new LegacyElementMembers Default { get; } = new LegacyElementMembers();
        LegacyElementMembers() : this(MemberElementSelector.Default) {}
        public LegacyElementMembers(IMemberElementSelector selector) : base(selector) {}
    }

    class LegacyMemberElementSelector : MemberElementSelector
    {
        public LegacyMemberElementSelector(ISerializationToolsFactory tools)
            : base(
                new MemberOption(new LegacyGetterFactory(tools), new LegacySetterFactory(tools, SetterFactory.Default)),
                ReadOnlyCollectionMemberOption.Default) {}
    }
}