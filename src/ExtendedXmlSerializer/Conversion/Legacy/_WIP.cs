using System.Reflection;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    class LegacyElements : Selector<MemberInfo, IElement>, IElementSelector
    {
        public static LegacyElements Default { get; } = new LegacyElements();
        LegacyElements() : this(DictionaryElementFactory.Default) {}

        public LegacyElements(params IOption<MemberInfo, IElement>[] options) : base(options) {}
    }
}
