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

using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public interface IWriteContext {
        IWriteContext Parent { get; }
        ProcessState State { get; }
        object Root { get; }
        object Instance { get; }
        ITypeDefinition Definition { get; }
        IImmutableList<MemberContext> Members { get; }
        MemberContext? Member { get; }
    }

    public class WriteContext : IWriteContext
    {
        public WriteContext(object root, ITypeDefinition definition)
            : this(null, ProcessState.Instance, root, root, definition, null, null) {}

        public WriteContext(IWriteContext parent, object instance, ITypeDefinition definition)
            : this(parent, ProcessState.Instance, parent.Root, instance, definition, null, null) {}

        public WriteContext(IWriteContext parent, IImmutableList<MemberContext> members)
            : this(parent, ProcessState.Members, parent.Root, parent.Instance, parent.Definition, members, null) {}

        public WriteContext(IWriteContext parent, MemberContext member)
            : this(parent, ProcessState.Member, parent.Root, parent.Instance, parent.Definition, parent.Members, member) {}

        public WriteContext(IWriteContext parent, ProcessState state, object root, object instance,
                            ITypeDefinition definition, IImmutableList<MemberContext> members,
                            MemberContext? member)
        {
            Parent = parent;
            State = state;
            Root = root;
            Instance = instance;
            Definition = definition;
            Members = members;
            Member = member;
        }

        public IWriteContext Parent { get; }
        public ProcessState State { get; }
        public object Root { get; }
        public object Instance { get; }
        public ITypeDefinition Definition { get; }
        public IImmutableList<MemberContext> Members { get; }
        public MemberContext? Member { get; }
    }
}