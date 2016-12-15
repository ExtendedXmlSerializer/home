// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.Services;

namespace ExtendedXmlSerialization.Cache
{
    class PropertieDefinition : IMemberDefinition
    {
        public PropertieDefinition(MemberInfo memberInfo, string name)
        {
            Metadata = memberInfo;
            Name = string.IsNullOrEmpty(name) ? memberInfo.Name : name;
            MemberType = TypeDefinitionCache.GetDefinition(memberInfo.GetMemberType());
            IsWritable = memberInfo.IsWritable();
            _getter = ObjectAccessors.CreatePropertyGetter(memberInfo);
            _propertySetter = Setters.Default.Get(memberInfo);
        }


        private readonly ObjectAccessors.PropertyGetter _getter;
        private readonly ObjectAccessors.PropertySetter _propertySetter;

        public string Name { get; private set; }
        public ITypeDefinition MemberType { get; }
        public MemberInfo Metadata { get; }
        public bool IsWritable { get; }
        public int Order { get; set; } = -1;
        public int MetadataToken { get; set; }
        public object GetValue(object obj)
        {
            return _getter(obj);
        }

        public void SetValue(object obj, object value)
        {
            _propertySetter?.Invoke(obj, value);
        }
    }
}
