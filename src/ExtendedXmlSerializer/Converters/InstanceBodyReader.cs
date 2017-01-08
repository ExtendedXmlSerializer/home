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

using System.Xml.Linq;
using ExtendedXmlSerialization.Converters.Members;
using ExtendedXmlSerialization.Converters.Read;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Converters
{
    class InstanceBodyReader : ReaderBase
    {
        private readonly IInstanceMembers _members;
        private readonly ITypes _types;
        private readonly IActivators _activators;

        public InstanceBodyReader(IInstanceMembers members, ITypes types, IActivators activators)
        {
            _members = members;
            _types = types;
            _activators = activators;
        }

        public override object Read(XElement element, Typed? hint = null)
        {
            var type = hint ?? _types.Get(element);
            var result = type.HasValue ? Create(element, type.Value) : null;
            return result;
        }

        protected virtual object Create(XElement element, Typed type)
        {
            var result = _activators.Activate<object>(type);
            OnRead(element, result, type);
            return result;
        }

        protected virtual void OnRead(XElement element, object result, Typed type)
        {
            var members = _members.Get(type);
            foreach (var child in element.Elements())
            {
                var member = members.Get(child.Name);
                if (member != null)
                {
                    Apply(result, member, member.Read(child, _types.Get(child)));
                }
            }
        }

        protected virtual void Apply(object instance, IMember member, object value)
        {
            if (value != null)
            {
                var assignable = member as IAssignableMember;
                assignable?.Set(instance, value);
            }
        }
    }
}