using System.Reflection;

namespace ExtendedXmlSerialization.ElementModel
{
    public class Root : ContainerElementBase, IRoot
    {
        public Root(IElementName name, TypeInfo declaredType) : base(name, declaredType) {}
    }
}
