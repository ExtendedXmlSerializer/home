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

using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Write.Services;

namespace ExtendedXmlSerialization.Write.Extensibility
{
    public abstract class ConfigurationWritingExtensionBase : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        protected ConfigurationWritingExtensionBase(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected IExtendedXmlSerializerConfig For(Type type) => _factory.GetConfiguration(type);

        public override bool Starting(IWriting writing)
        {
            var current = writing.Current;
            if (current.Instance != null)
            {
                var type = current.Instance.GetType();
                var configuration = For(type);
                if (configuration != null)
                {
                    if (current.Member != null)
                    {
                        switch (current.State)
                        {
                            case WriteState.MemberValue:
                                return StartingMemberValue(writing, configuration, current.Instance,
                                                           current.Member.GetValueOrDefault());

                            default:
                                return StartingMember(writing, configuration, current.Instance,
                                                      current.Member.GetValueOrDefault());
                        }
                    }

                    var result = current.Members != null
                        ? StartingMembers(writing, configuration, current.Instance, current.Members)
                        : StartingInstance(writing, configuration, current.Instance);
                    return result;
                }
            }

            var starting = base.Starting(writing);
            return starting;
        }

        protected virtual bool StartingInstance(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                                object instance) => true;

        protected virtual bool StartingMembers(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                               object instance, IImmutableList<MemberInfo> members) => true;

        protected virtual bool StartingMember(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                              object instance, MemberContext member) => true;

        protected virtual bool StartingMemberValue(IWriting writing, IExtendedXmlSerializerConfig configuration,
                                                   object instance, MemberContext member) => true;
    }
}