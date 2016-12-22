using System;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Processing.Write
{
    internal interface IVersionLocator : IParameterizedSource<Type, int?> {}

    class VersionLocator : IVersionLocator
    {
        private readonly ISerializationToolsFactory _factory;
        public VersionLocator(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        public int? Get(Type parameter) => _factory.GetConfiguration(parameter)?.Version;
    }
}