using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Configuration
{
    public interface IExtendedXmlTypeMigrator
    {
        IEnumerable<Action<XElement>> GetAllMigrations();
    }
}