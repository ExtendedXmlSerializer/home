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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Conversion.Members
{
    sealed class Members : IMembers
    {
        private readonly ImmutableArray<IMember> _items;
        private readonly IDictionary<XName, IMember> _lookup;

        public Members(IEnumerable<IMember> items) : this(items.ToImmutableArray()) {}
        public Members(ImmutableArray<IMember> items) : this(items, items.ToDictionary(x => x.Name)) {}

        public Members(ImmutableArray<IMember> items, IDictionary<XName, IMember> lookup)
        {
            _items = items;
            _lookup = lookup;
        }

        public IMember Get(XName parameter)
        {
            IMember result;
            return _lookup.TryGetValue(parameter, out result) ? result : null;
        }

        public IEnumerator<IMember> GetEnumerator() => ((IEnumerable<IMember>) _items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}