using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.NewConfiguration
{
    public interface IExtendedXmlSerializerTypeMigrator
    {
        IEnumerable<Action<XElement>> GetAllMigrations();
    }
}