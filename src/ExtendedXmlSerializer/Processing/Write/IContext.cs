using System;
using System.Collections.Generic;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Processing.Write
{
    public interface IContextProvider : IAlteration<IContext> {}

    public interface IContext : IContent
    {
        /*Type InstanceType { get; }

        object Content { get; }*/
    }


    public interface IContextWriter
    {
        void Write(IContext context);
    }

    abstract class ContextBase : ContextBase<object>
    {
        protected ContextBase(object content, Type instanceType) : base(content, instanceType) {}
    }

    public interface IContent
    {
        Type ContentType { get; }
        object Content { get; }
    }

    abstract class ContextBase<T> : IContext
    {
        protected ContextBase(T content, Type instanceType)
        {
            Content = content;
            InstanceType = instanceType;
        }

        public T Content { get; }

        public Type InstanceType { get; }

        object IContent.Content => Content;
    }

    class RootContext : ContextBase
    {
        public RootContext(object content, Type instanceType) : base(content, instanceType) {}
    }

    /*class PrimitiveInstruction : IInstruction
    {
        private readonly IContextReader _reader;
        private readonly IContextWriter _writer;

        public PrimitiveInstruction(IContextReader reader, IContextWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public void Execute(IContext context)
        {
            var instance = _reader.Read(context);
            _writer.Write(new PrimitiveContext(instance));
        }
    }*/

    /*class PrimitiveContext : IContext
    {
        private readonly object _instance;
        public PrimitiveContext(object instance)
        {
            _instance = instance;
        }

        public object Get() => _instance;
    }*/

    class PrimitiveProvider : IContextProvider
    {
        readonly private static IDictionary<TypeCode, string> Codes = new Dictionary<TypeCode, string>
                                                                      {
                                                                          {TypeCode.Boolean, "boolean"},
                                                                          {TypeCode.Char, "char"},
                                                                          {TypeCode.SByte, "byte"},
                                                                          {TypeCode.Byte, "unsignedByte"},
                                                                          {TypeCode.Int16, "short"},
                                                                          {TypeCode.UInt16, "unsignedShort"},
                                                                          {TypeCode.Int32, "int"},
                                                                          {TypeCode.UInt32, "unsignedInt"},
                                                                          {TypeCode.Int64, "long"},
                                                                          {TypeCode.UInt64, "unsignedLong"},
                                                                          {TypeCode.Single, "float"},
                                                                          {TypeCode.Double, "double"},
                                                                          {TypeCode.Decimal, "decimal"},
                                                                          {TypeCode.DateTime, "dateTime"},
                                                                          {TypeCode.String, "string"},
                                                                      };

        readonly private static IDictionary<Type, string> Other = new Dictionary<Type, string>
                                                                  {
                                                                      {typeof(Guid), "guid"},
                                                                      {typeof(TimeSpan), "TimeSpan"},
                                                                  };

        public static PrimitiveProvider Default { get; } = new PrimitiveProvider();
        PrimitiveProvider() {}

        public IContext Get(IContext parameter)
        {
            //var type = parameter.Instance.GetType();
            var type = parameter.InstanceType;
            var code = Type.GetTypeCode(type);
            // var result = Codes.ContainsKey(code) || Other.ContainsKey(type);
            string name;
            var found = Codes.TryGetValue(code, out name) || Other.TryGetValue(type, out name);
            if (found)
            {
                return new PrimitiveContext(parameter.Read(parameter));
            }
            return null;
        }
    }

    public interface IWriting
    {
        void Write(object value);
    }

    // public interface IContextSelector : IParameterizedSource<object, IContext> {}

    class ContextSelector : IContextProvider
    {
        private readonly IContextProvider[] _providers;

        public ContextSelector(params IContextProvider[] providers)
        {
            _providers = providers;
        }

        public IContext Get(IContext parameter)
        {
            // var instance = parameter.Read(parameter);
            // var type = parameter.GetType();
            foreach (var provider in _providers)
            {
                var context = provider.Get(parameter);
                if (context != null)
                {
                    return context;
                }
            }
            // throw;
            return null;
        }
    }

    public struct WriteContext
    {
        public WriteContext(IContextWriter writer, object currentInstance) {}
    }

    class DocumentWriter : IContextWriter
    {
        public void Write(IContext context)
        {
            
        }
    }
}