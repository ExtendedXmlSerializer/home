using System.Reflection;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public struct MemberInformation
    {
        public MemberInformation(MemberInfo metadata, TypeInfo memberType, bool assignable)
        {
            Metadata = metadata;
            MemberType = memberType;
            Assignable = assignable;
        }

        public MemberInfo Metadata { get; }
        public TypeInfo MemberType { get; }
        public bool Assignable { get; }
    }

    public interface IMemberElementFactory : IParameterizedSource<MemberInformation, IMemberElement> {}

    public class ReadOnlyCollectionMemberConverterFactory : ConverterFactoryBase<IReadOnlyCollectionMemberElement>
    {
        private readonly IEnumeratingReader _reader;
        private readonly IConverter _converter;

        public ReadOnlyCollectionMemberConverterFactory(IConverter converter, IEnumeratingReader reader)
        {
            _converter = converter;
            _reader = reader;
        }

        protected override IConverter Create(IReadOnlyCollectionMemberElement parameter)
            => new MemberConverter(_reader, _converter);
    }
}