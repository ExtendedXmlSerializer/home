using System.Reflection;

namespace ExtendedXmlSerialization.ElementModel
{
    public class Root : ContainerElementBase, IRoot
    {
        public Root(TypeInfo declaredType) : base(declaredType) {}
    }
}
