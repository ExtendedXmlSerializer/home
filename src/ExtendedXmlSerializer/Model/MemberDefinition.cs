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
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    class MemberDefinition : IMemberDefinition
    {
        private readonly ObjectAccessors.PropertyGetter _getter;
        private readonly ObjectAccessors.PropertySetter _propertySetter;

        public MemberDefinition(MemberInfo memberInfo, string name)
        {
            Metadata = memberInfo;
            Name = string.IsNullOrEmpty(name) ? memberInfo.Name : name;
            TypeDefinition = TypeDefinitions.Default.Get(memberInfo.GetMemberType());
            IsWritable = memberInfo.IsWritable();
            _getter = ObjectAccessors.CreatePropertyGetter(memberInfo);
            _propertySetter = IsWritable ? ObjectAccessors.CreatePropertySetter(memberInfo.DeclaringType, memberInfo.Name) : TypeDefinition.Add;
        }

        public string Name { get; }
        public ITypeDefinition TypeDefinition { get; }
        public Type Type => TypeDefinition.Type;
        public MemberInfo Metadata { get; }
        public bool IsWritable { get; }
        public int Order { get; set; } = -1;

        public object GetValue(object obj) => _getter(obj);
        public void SetValue(object obj, object value) => _propertySetter?.Invoke(obj, value);
    }
}