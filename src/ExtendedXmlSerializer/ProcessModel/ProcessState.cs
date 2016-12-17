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
using System.IO;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.ProcessModel.Write;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.ProcessModel
{
    /*public interface IContextAware
    {
        IContext Current { get; }
    }

    public interface IContextController : IContextAware, IEnumerable<IContext>
    {
        void MakeCurrent(IContext current);
        void Undo();
    }

    class ContextController : IContextController
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

        /*public void Dispose()
        {
            
        }#1#
    }*/

    public interface IProcess : /*IContextAware,*/ IDisposable/*, ICommand<IInstruction>*/, IServiceProvider {}

    public interface ICommand<in T>
    {
        void Execute(T parameter);
    }

/*
    public class FirstConditionalCommand<T> : ICommand<T>
    {
        private readonly IEnumerable<IConditionalCommand<T>> _commands;
        private readonly Lazy<IConditionalCommand<T>[]> _deferred;
        public FirstConditionalCommand(IEnumerable<IConditionalCommand<T>> commands)
        {
            _commands = commands;
            _deferred = new Lazy<IConditionalCommand<T>[]>(Commands);
        }

        private IConditionalCommand<T>[] Commands() => _commands.ToArray();

        public void Execute(T parameter)
        {
            var commands = _deferred.Value;
            var length = commands.Length;
            for (var i = 0; i < length; i++)
            {
                var command = commands[i];
                if (command.IsSatisfiedBy(parameter))
                {
                    command.Execute(parameter);
                    return;
                }
            }
        }
    }
*/

    public class ConditionalCommand<T> : ConditionalCommandBase<T>
    {
        private readonly ISpecification<T> _specification;
        private readonly ICommand<T> _command;

        public ConditionalCommand(ISpecification<T> specification, ICommand<T> command)
        {
            _specification = specification;
            _command = command;
        }

        public override bool IsSatisfiedBy(T parameter) => _specification.IsSatisfiedBy(parameter);

        public override void Execute(T parameter) => _command.Execute(parameter);
    }

    public interface IConditionalCommand<in T> : ISpecification<T>, ICommand<T> { }
    public abstract class ConditionalCommandBase<T> : IConditionalCommand<T>
    {
        public abstract bool IsSatisfiedBy(T parameter);
        public abstract void Execute(T parameter);
    }


    public static class Extensions
    {
        // public static IScope CreateScope<T>(this IScopeFactory @this, T instance) => @this.Create(instance);
        // public static IScope CreateScope<T>(this IScope @this, T instance) => @this.Create(instance);

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

/*
    public abstract class ScopeBase<T> : IScope<T>
    {
        protected ScopeBase(IContext parent, T instance, ITypeDefinition definition)
        {
            Parent = parent;
            Instance = instance;
            Definition = definition;
        }

        public IContext Parent { get; }

        public T Instance { get; }
        object IContext.Instance => Instance;

        public ITypeDefinition Definition { get; }

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
*/

    /*public interface IScope<out T> : IScope
    {
        new T Instance { get; }
    }*/
}