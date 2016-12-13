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
using System.Reflection;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    sealed class DictionaryEntryElementInformation : ApplicationElementInformationBase
    {
        public static DictionaryEntryElementInformation Default { get; } = new DictionaryEntryElementInformation();
        DictionaryEntryElementInformation() : base(ExtendedXmlSerializer.Item) {}
    }

    sealed class DictionaryKeyElementInformation : ApplicationElementInformationBase
    {
        public static DictionaryKeyElementInformation Default { get; } = new DictionaryKeyElementInformation();
        DictionaryKeyElementInformation() : base(ExtendedXmlSerializer.Key) {}
    }


    sealed class DictionaryValueElementInformation : ApplicationElementInformationBase
    {
        public static DictionaryValueElementInformation Default { get; } = new DictionaryValueElementInformation();
        DictionaryValueElementInformation() : base(ExtendedXmlSerializer.Value) {}
    }

    sealed class ItemElements : WeakCacheBase<ITypeDefinition, IElementInformation>
    {
        public static ItemElements DefaultElement { get; } = new ItemElements();
        ItemElements() {}

        protected override IElementInformation Callback(ITypeDefinition key)
            => new ItemElementInformation(TypeDefinitionCache.GetDefinition(key.GenericArguments[0]));
    }

    sealed class ItemElementInformation : ElementInformationBase
    {
        private readonly ITypeDefinition _definition;
        private readonly bool _isInterface;

        public ItemElementInformation(ITypeDefinition definition)
            : this(definition, definition.Type.GetTypeInfo().IsInterface) {}

        public ItemElementInformation(ITypeDefinition definition, bool isInterface)
        {
            _definition = definition;
            _isInterface = isInterface;
        }

        public override Type GetType(IWriteContext context) => context.Instance.GetType();

        public override string GetName(IWriteContext context)
        {
            var type = _isInterface ? context.Definition : _definition;
            var result = type.Name;
            return result;
        }
    }

    class RootElementInformation : ElementInformation
    {
        public static RootElementInformation Default { get; } = new RootElementInformation();
        RootElementInformation() : base(context => context.Root.GetType()) {}
    }

    class MemberElementInformation : ElementInformation
    {
        public static MemberElementInformation Instance { get; } = new MemberElementInformation();

        MemberElementInformation()
            : base(context => context.Member?.Metadata.DeclaringType, context => context.Member?.DisplayName) {}
    }


    public interface IElementInformation
    {
        Type GetType(IWriteContext context);
        string GetName(IWriteContext context);
    }

    abstract class ApplicationElementInformationBase : FixedElementInformation
    {
        readonly private static Type Type = typeof(IExtendedXmlSerializer);
        protected ApplicationElementInformationBase(string name) : base(Type, name) {}
    }

    class FixedElementInformation : ElementInformationBase
    {
        private readonly Type _type;
        private readonly string _name;

        public FixedElementInformation(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        public override Type GetType(IWriteContext context) => _type;

        public override string GetName(IWriteContext context) => _name;
    }

    abstract class ElementInformationBase : IElementInformation
    {
        public abstract Type GetType(IWriteContext context);
        public abstract string GetName(IWriteContext context);
    }

    class ElementInformation : ElementInformationBase
    {
        private readonly Func<IWriteContext, Type> _type;
        private readonly Func<IWriteContext, string> _name;

        public ElementInformation(Func<IWriteContext, Type> type) : this(type, context => context.Definition.Name) {}

        public ElementInformation(Func<IWriteContext, Type> type, Func<IWriteContext, string> name)
        {
            _type = type;
            _name = name;
        }

        public override Type GetType(IWriteContext context) => _type(context);

        public override string GetName(IWriteContext context) => _name(context);
    }
}