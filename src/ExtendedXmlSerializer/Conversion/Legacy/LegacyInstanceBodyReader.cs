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
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class LegacyInstanceBodyReader : InstanceBodyReader
    {
        private readonly ISerializationToolsFactory _tools;

        public LegacyInstanceBodyReader(ISerializationToolsFactory tools, IInstanceMembers members) : base(members)
        {
            _tools = tools;
        }

        protected override object Activate(IReadContext context)
        {
            var result = base.Activate(context);
            var type = context.ReferencedType;
            var configuration = _tools.GetConfiguration(type);
            if (configuration != null && configuration.IsObjectReference)
            {
                var prefix = context.ReferencedType.Type.FullName + Defaults.Underscore;
                var refId = context[ReferenceProperty.Default];
                var references = context.Get<ReadReferences>();
                if (!string.IsNullOrEmpty(refId))
                {
                    var key = prefix + refId;
                    if (references.ContainsKey(key))
                    {
                        return references[key];
                    }
                    references.Add(key, result);
                }
                else
                {
                    var objectId = context[IdentifierProperty.Default];
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        var key = prefix + objectId;
                        if (references.ContainsKey(key))
                        {
                            return references[key];
                        }
                        references.Add(key, result);
                    }
                }
            }
            return result;
        }
    }
}