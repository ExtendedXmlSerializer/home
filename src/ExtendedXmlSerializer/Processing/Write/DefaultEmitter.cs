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

using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
/*
        class ReferenceAwareEmitter : IEmitter
        {
            private readonly IWriter _writer;
    
            public ReferenceAwareEmitter(IWriter writer)
            {
                _writer = writer;
            }
    
            public void Execute(IEntity parameter) => Execute(parameter, null);
    
            private void Execute(IEntity parameter, IEntity context)
            {
                var primitive = parameter as IPrimitive;
                if (primitive != null)
                {
                    using (_writer.Begin(context ?? primitive))
                    {
                        ApplyType(primitive);
                        _writer.Emit(primitive.Instance);
                    }
                    return;
                }
    
                var instance = parameter as IObject;
                if (instance != null)
                {
                    EmitObject(context, instance);
                    return;
                }
    
                var identity = parameter as IUniqueObject;
                if (identity != null)
                {
                    var content = identity.Content;
                    using (_writer.Begin(context ?? content))
                    {
                        var force = Apply(content, context);
                        ApplyType(content, force);
                        _writer.Emit(new IdentityProperty(identity.Id));
    
                        foreach (var o in content)
                        {
                            Execute(o, o);
                        }
                    }
                    return;
                }
    
                var reference = parameter as IReference;
                if (reference != null)
                {
                    using (_writer.Begin(context ?? reference))
                    {
                        var content = reference.Entity;
                        var o = content.Content;
                        ApplyType(o, o.ActualType.Type != context.DeclaredType.Type);
                        _writer.Emit(new ObjectReferenceProperty(content.Id));
                    }
                    return;
                }
    
                var container = parameter as IContext;
                if (container != null)
                {
                    Execute(container.Entity, context ?? container);
                }
            }
    
            private void EmitObject(IEntity context, IObject instance)
            {
                using (_writer.Begin(context ?? instance))
                {
                    var force = Apply(instance, context);
                    ApplyType(instance, force);
    
                    foreach (var o in instance)
                    {
                        Execute(o, o);
                    }
                }
            }
    
            private static bool Apply(IInstance instance, IEntity context)
            {
                var ignore = instance is IEnumerableObject || instance is IDictionaryEntry;
                if (!ignore)
                {
                    return true;
                }
    
                var result = ((context as IMember)?.Definition.IsWritable ?? false) && Different(instance);
                return result;
            }
    
            private void ApplyType(IInstance node) => ApplyType(node, Different(node));
    
            private static bool Different(IInstance node) => node.ActualType.Type != node.DeclaredType.Type;
    
            private void ApplyType(IInstance node, bool force)
            {
                if (force)
                {
                    _writer.Emit(new TypeProperty(node.ActualType.Type));
                }
            }
        }
    */

    class DefaultEmitter : IEmitter
    {
        private readonly IWriter _writer;

        public DefaultEmitter(IWriter writer)
        {
            _writer = writer;
        }

        public void Execute(IContext parameter)
        {
            var primitive = parameter.Entity as IPrimitive;
            if (primitive != null)
            {
                using (_writer.Begin(parameter))
                {
                    ApplyType(parameter);
                    _writer.Emit(primitive.Value);
                }
                return;
            }

            var instance = parameter.Entity as IObject;
            if (instance != null)
            {
                using (_writer.Begin(parameter))
                {
                    ApplyType(parameter);

                    foreach (var o in instance.Members)
                    {
                        Execute(o);
                    }

                    var enumerable = instance as IEnumerableObject;
                    if (enumerable != null)
                    {
                        foreach (var item in enumerable.Items)
                        {
                            Execute(item);
                        }
                    }
                }
                return;
            }
        }

        private static bool Apply(IContext context)
        {
            var ignore = context.Entity is IEnumerableObject || context is IItem;
            if (!ignore)
            {
                return true;
            }

            var member = context as IMember;
            var result = member != null && member.Definition.IsWritable && context.Entity.Type != member.Definition.Type;
            return result;
        }

        private void ApplyType(IContext context)
        {
            if (Apply(context))
            {
                _writer.Emit(new TypeProperty(context.Entity.Type));
            }
        }
    }
}