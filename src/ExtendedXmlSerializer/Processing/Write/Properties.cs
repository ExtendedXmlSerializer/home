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

using System.Collections.Generic;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    sealed class Properties : WeakCacheBase<IElement, IProperty>, IIdentities
    {
        private readonly IIdentityLocator _locator;
        readonly ISet<object> _watching = new HashSet<object>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public Properties(IIdentityLocator locator)
        {
            _locator = locator;
        }

        protected override IProperty Create(IElement parameter)
        {
            var instance = parameter.Content.Instance;
            var id = _locator.Get(instance);
            if (id != null)
            {
                var identity = _watching.Contains(instance) ? parameter is IItem : _generator.For(instance).FirstEncounter;
                var result = identity ? (IProperty) new IdentityProperty(id) : new ObjectReferenceProperty(id);
                return result;
            }
            return null;
        }

        public void Track(object instance)
        {
            if (!_generator.Contains(instance))
            {
                _watching.Add(instance);
            }
        }

        public void Release(object instance) => _watching.Remove(instance);
    }
}