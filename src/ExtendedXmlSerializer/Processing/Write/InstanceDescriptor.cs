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
using ExtendedXmlSerialization.Model;

namespace ExtendedXmlSerialization.Processing.Write
{
    public struct InstanceDescriptor
    {
        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;

        public InstanceDescriptor(object instance)
            : this(instance, Definition(instance.GetType())) {}

        public InstanceDescriptor(object instance, ITypeDefinition declaredType)
            : this(instance, declaredType, declaredType.Name) {}

        public InstanceDescriptor(object instance, ITypeDefinition declaredType,
                                  string name) : this(instance, declaredType, declaredType.For(instance), name) {}

        public InstanceDescriptor(object instance, ITypeDefinition declaredType, ITypeDefinition actualType) : this(instance, declaredType, actualType, actualType.Name) {}

        public InstanceDescriptor(object instance, ITypeDefinition declaredType, ITypeDefinition actualType,
                                  string name)
        {
            Instance = instance;
            DeclaredType = declaredType;
            ActualType = actualType;
            Name = name;
        }

        public object Instance { get; }
        public ITypeDefinition DeclaredType { get; }
        public ITypeDefinition ActualType { get; }
        public string Name { get; }
    }
}