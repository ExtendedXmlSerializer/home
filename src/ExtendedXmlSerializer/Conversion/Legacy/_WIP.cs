using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    class LegacyElements : Selector<MemberInfo, IElement>, IElementFactory
    {
        public static ElementModel.Elements Default { get; } = new ElementModel.Elements();
        LegacyElements() : this(DictionaryElementFactory.Default) {}

        public LegacyElements(params IOption<MemberInfo, IElement>[] options) : base(options) {}
    }
}
