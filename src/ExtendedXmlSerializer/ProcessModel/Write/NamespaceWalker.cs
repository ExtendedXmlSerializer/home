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
using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    class NamespaceWalker : ObjectWalkerBase<object, IEnumerable<Uri>>
    {
        private readonly INamespaceLocator _locator;
        private readonly ISpecification<Type> _primitive;

        public NamespaceWalker(object root, INamespaceLocator locator)
            : this(root, locator, IsPrimitiveSpecification.Default) {}

        public NamespaceWalker(object root, INamespaceLocator locator, ISpecification<Type> primitive) : base(root)
        {
            _locator = locator;
            _primitive = primitive;
        }

        protected override IEnumerable<Uri> Select(object input)
        {
            var type = input as Type;
            if (type != null)
            {
                foreach (var info in SerializableMembers.Default.Get(type))
                {
                    var memberType = info.MemberType;
                    if (info.IsWritable && !_primitive.IsSatisfiedBy(memberType))
                    {
                        Schedule(memberType);
                    }

                    var definition = TypeDefinitionCache.GetDefinition(memberType);
                    if (definition.IsDictionary)
                    {
                        foreach (var argument in definition.GenericArguments)
                        {
                            if (!_primitive.IsSatisfiedBy(argument))
                            {
                                Schedule(argument);
                            }
                        }
                    }
                    else if (definition.IsEnumerable)
                    {
                        var elementType = ElementTypeLocator.Default.Locate(memberType);
                        if (elementType != null)
                        {
                            Schedule(elementType);
                        }
                    }
                }
                yield return _locator.Locate(type);
            }
            else
            {
                var dictionary = input as IDictionary;
                if (dictionary != null)
                {
                    var arguments = TypeDefinitionCache.GetDefinition(dictionary.GetType()).GenericArguments;
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        var key = entry.Key.GetType();
                        if (key != arguments[0])
                        {
                            Schedule(key);
                            Schedule(entry.Key);
                        }

                        var value = entry.Value.GetType();
                        if (value != arguments[1])
                        {
                            Schedule(value);
                            Schedule(entry.Value);
                        }
                    }
                }
                else if (Arrays.Default.Is(input))
                {
                    var inputType = input.GetType();
                    var elementType = ElementTypeLocator.Default.Locate(inputType);

                    Schedule(elementType);
                    foreach (var element in Arrays.Default.AsArray(input))
                    {
                        var instanceType = element.GetType();
                        if (instanceType != elementType)
                        {
                            Schedule(instanceType);
                            Schedule(element);
                        }
                    }
                }
                else
                {
                    foreach (var context in SerializableMembers.Default.Get(input.GetType()))
                    {
                        if (context.IsWritable)
                        {
                            if (!_primitive.IsSatisfiedBy(context.MemberType))
                            {
                                Schedule(context.MemberType);
                            }
                            var value = context.Value(input);
                            if (value != DefaultValues.Default.Get(context.MemberType))
                            {
                                var instanceType = value.GetType();
                                if (instanceType != context.MemberType)
                                {
                                    Schedule(instanceType);
                                    Schedule(value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}