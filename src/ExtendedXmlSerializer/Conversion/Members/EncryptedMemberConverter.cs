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
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public class EncryptedMemberConverter : IAssignableMemberConverter
    {
        private readonly IPropertyEncryption _encryption;
        private readonly IAssignableMemberConverter _member;

        public EncryptedMemberConverter(IPropertyEncryption encryption, IAssignableMemberConverter member)
        {
            _encryption = encryption;
            _member = member;
        }

        public object Read(IReadContext context)
        {
            var element = context.Get<XElement>();
            element.Value = _encryption.Decrypt(element.Value);
            return _member.Read(context);
        }

        public void Write(IWriteContext context, object instance)
        {
            _member.Write(context, instance);
        }

        public void Set(object instance, object value) => _member.Set(instance, value);

        public string Name => _member.Name;

        public Typing ReferencedType => _member.ReferencedType;

        public bool Assignable => _member.Assignable;

        public MemberInfo Metadata => _member.Metadata;

        public Typing MemberType => _member.MemberType;
    }
}