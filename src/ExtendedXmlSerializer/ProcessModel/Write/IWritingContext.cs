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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public interface ISerialization : IProcess, IScopeFactory, IWriteContextAware
    {
        /*void Emit(object instance);
        void Emit(IProperty property);
        void Emit(Type type);*/
    }

    public interface IWriteContextAware
    {
        IContext Current { get; }
    }

    public interface IContextMonitor : IWriteContextAware, IEnumerable<IContext>
    {
        void MakeCurrent(IContext current);
        void Undo();
    }

    class ContextMonitor : IContextMonitor
    {
        public IContext Current { get; private set; }
        public void MakeCurrent(IContext current) => Current = current;
        public void Undo() => Current = Current?.Parent;

        public IEnumerator<IContext> GetEnumerator()
        {
            var current = Current;
            while (true)
            {
                yield return current;
                if (current.Parent != null)
                {
                    current = current.Parent;
                    continue;
                }
                break;
            }
            // return _stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /*public IWriteContext Parent => Current.Parent;

        public object Instance => Current.Instance;

        public ITypeDefinition Definition => Current.Definition;*/

        /*public IImmutableList<MemberContext> Members => _current.Members;

        public MemberContext? Member => _current.Member;*/

        /*public void Dispose()
        {
            
        }*/
    }
}