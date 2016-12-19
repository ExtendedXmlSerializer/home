using ExtendedXmlSerialization.Model;

namespace ExtendedXmlSerialization.Processing
{
    public static class ValueServices
    {
        public static string AsString(object instance)
            => PrimitiveValueTools.SetPrimitiveValue(instance);

        public static object Convert(this ITypeDefinition @this, string instance)
            => PrimitiveValueTools.GetPrimitiveValue(instance, @this);
    }
}
