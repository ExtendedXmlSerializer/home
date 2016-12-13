using System;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Elements;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public static class Extensions
    {
        public static object Value(this IWriteContext @this) => @this.Member?.Value(@this.Instance);

        public static IDisposable NewInstance<T>(this ISerialization @this, T instance, IElementInformation information) => @this.New(instance, information);

        public static void Attach(this ISerialization @this, IProperty property) => AttachedProperties.Default.Attach(@this.Current.Instance, property);

        public static IImmutableList<IProperty> GetProperties(this ISerialization @this)
        {
            var list = AttachedProperties.Default.GetProperties(@this.Current.Instance);
            var result = list.ToArray().ToImmutableList();
            list.Clear();
            return result;
        }
    }
}