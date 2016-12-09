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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Elements;

namespace ExtendedXmlSerialization.Services.Services
{
    public class Namespaces : INamespaces
    {
        private readonly INamespaceLocator _locator;
        private readonly Func<IPrefixGenerator> _generator;
        private readonly INamespace _root;
        private readonly IImmutableList<INamespace> _namespaces;

        public Namespaces(INamespaceLocator locator, INamespace root, params INamespace[] namespaces)
            : this(locator, () => new PrefixGenerator(), root, namespaces) {}

        public Namespaces(INamespaceLocator locator, Func<IPrefixGenerator> generator, INamespace root,
                          params INamespace[] namespaces)
        {
            _locator = locator;
            _generator = generator;
            _root = root;
            _namespaces = root.Append(namespaces).ToImmutableList();
        }

        public IImmutableList<INamespace> Get(object parameter)
        {
            var result = Yield(parameter).Distinct().OrderBy(x => x.Prefix).ToImmutableList();
            return result;
        }

        INamespace Lookup(Uri candidate)
        {
            foreach (var ns in _namespaces)
            {
                if (ns.Identifier == candidate)
                {
                    return ns;
                }
            }
            return null;
        }

        private IEnumerable<INamespace> Yield(object parameter)
        {
            yield return _root;
            var generator = _generator();
            var root = _locator.Get(parameter);
            foreach (
                var candidate in new NamespaceWalker(parameter, _locator).SelectMany(locations => locations).Distinct())
            {
                if (candidate != root)
                {
                    yield return Lookup(candidate) ?? new Namespace(generator.Generate(candidate), candidate);
                }
            }
        }
    }
}