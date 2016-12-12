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
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Instructions;

namespace ExtendedXmlSerialization.Plans
{
    public class PlanSelector : IPlan
    {
        readonly ICollection<IPlan> _providers;

        readonly WeakCache<Type, ICollection<IAssignableInstruction>> _placeholders =
            new WeakCache<Type, ICollection<IAssignableInstruction>>(key => new HashSet<IAssignableInstruction>());

        public PlanSelector(ICollection<IPlan> providers)
        {
            _providers = providers;
        }

        public IInstruction For(Type type)
        {
            var contains = _placeholders.Contains(type);
            var placeholders = _placeholders.Get(type);
            if (contains)
            {
                var result = new PlaceholderInstruction();
                placeholders.Add(result);
                return result;
            }

            try
            {
                foreach (var provider in _providers)
                {
                    var result = provider.For(type);
                    if (result != null)
                    {
                        foreach (var instruction in placeholders)
                        {
                            instruction.Assign(result);
                        }
                        return result;
                    }
                }
            }
            finally
            {
                placeholders.Clear();
            }
            return null;
        }

        class PlaceholderInstruction : IAssignableInstruction
        {
            private IInstruction _instruction;
            public void Execute(IServiceProvider services) => _instruction?.Execute(services);

            public void Assign(IInstruction instruction) => _instruction = instruction;
        }
    }
}