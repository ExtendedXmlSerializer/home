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
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.ProcessModel.Write;

namespace ExtendedXmlSerialization.ProcessModel
{
    /*public enum ProcessState
    {
        // Root,
        Instance,
        Members,
        Member/*,
        MemberValue#1#
    }*/

    public interface IProcess : IDisposable, ICommand<IInstruction>, IServiceProvider {}

    public interface ICommand<in T>
    {
        void Execute(T parameter);
    }

    public interface IScope : IScopeFactory, IContext, IInstruction, IDisposable
    {
        // IScope Create(object instance);
    }

    public interface IContext
    {
        IContext Parent { get; }
        object Instance { get; }
        ITypeDefinition Definition { get; }
    }

    public static class Extensions
    {
        public static IScope CreateScope<T>(this IScopeFactory @this, T instance) => @this.Create(instance);
        public static IScope CreateScope<T>(this IScope @this, T instance) => @this.Create(instance);

        /*public static Context Current(this ISerializationContext @this, IContext context) => new Context(@this, context);

        public class Context :  IDisposable
        {
            private readonly ISerializationContext _serialization;
            
            public Context(ISerializationContext serialization, IContext context)
            {
                _serialization = serialization;
                _serialization.MakeCurrent(context);
            }

            public void Dispose() => _serialization.Undo();
        }*/
    }

    public abstract class ScopeBase<T> : IScope<T>
    {
        private readonly IScopeFactory _factory;

        protected ScopeBase(IScope parent, T instance, ITypeDefinition definition) : this(parent, parent, instance, definition) {}

        protected ScopeBase(IScopeFactory factory, IContext parent, T instance, ITypeDefinition definition)
        {
            _factory = factory;
            Parent = parent;
            Instance = instance;
            Definition = definition;
        }

        public IContext Parent { get; }

        public T Instance { get; }
        object IContext.Instance => Instance;

        public ITypeDefinition Definition { get; }

        public virtual IScope Create(object instance) => _factory.Create(instance);

        public abstract void Execute(IProcess parameter);

        ~ScopeBase()
        {
            OnDispose(false);
        }

        public void Dispose()
        {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose(bool disposing) {}
    }

    public interface IScope<out T> : IScope
    {
        new T Instance { get; }
    }
}