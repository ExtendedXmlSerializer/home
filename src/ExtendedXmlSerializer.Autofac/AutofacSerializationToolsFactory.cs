using System;
using Autofac;

namespace ExtendedXmlSerialization.Autofac
{
    public class AutofacSerializationToolsFactory : ISerializationToolsFactory
    {
        private readonly IComponentContext _context;

        public AutofacSerializationToolsFactory(IComponentContext context)
        {
            _context = context;
        }
        public IMigrationMap GetMigrationMap(Type type)
        {
            var genericTyep = typeof(IMigrationMap<>).MakeGenericType(type);
            object result;
            if (_context.TryResolve(genericTyep, out result))
            {
                return result as IMigrationMap;
            }
            return null;
        }

        public ICustomSerializator GetCustomSerializer(Type type)
        {
            var genericTyep = typeof(ICustomSerializator<>).MakeGenericType(type);
            object result;
            if (_context.TryResolve(genericTyep, out result))
            {
                return result as ICustomSerializator;
            }
            return null;
        }
    }
}
