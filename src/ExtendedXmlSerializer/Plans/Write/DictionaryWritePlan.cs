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
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Extensibility.Write;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Instructions.Write;

namespace ExtendedXmlSerialization.Plans.Write
{
    class DictionaryWritePlan : ConditionalPlanBase
    {
        private readonly IPlan _selector, _members;

        public DictionaryWritePlan(IPlan selector, IPlan members)
            : base(type => TypeDefinitionCache.GetDefinition(type).IsDictionary)
        {
            _selector = selector;
            _members = members;
        }

        protected override IInstruction Plan(Type type)
        {
            var arguments = TypeDefinitionCache.GetDefinition(type).GenericArguments;
            if (arguments.Length != 2)
            {
                throw new InvalidOperationException(
                          $"Attempted to write type '{type}' as a dictionary, but it does not have enough generic arguments.");
            }
            var key =
                new EmitInstanceInstruction(new ApplicationElementProvider((ns, o) => new DictionaryKeyElement(ns)),
                                            _selector.For(arguments[0]));
            var value =
                new EmitInstanceInstruction(new ApplicationElementProvider((ns, o) => new DictionaryValueElement(ns)),
                                            _selector.For(arguments[1]));

            var template = new EmitInstanceInstruction(
                               new ApplicationElementProvider((ns, o) => new DictionaryItemElement(ns)),
                               new ExtensionEnabledInstruction(new EmitDictionaryPairInstruction(
                                                                   new ExtensionEnabledInstruction(key),
                                                                   new ExtensionEnabledInstruction(value)))
                           );

            var result = new CompositeInstruction(_members.For(type), new EmitDictionaryItemsInstruction(template));
            return result;
        }
    }
}