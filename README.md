ExtendedXmlSerializer
=====================

[![Build status](https://ci.appveyor.com/api/projects/status/ub776yxp0nj535qp?svg=true)](https://ci.appveyor.com/project/ExtendedXmlSerializer/extendedxmlserializer) [![Nuget](https://img.shields.io/nuget/v/ExtendedXmlSerializer.svg)](https://www.nuget.org/packages/ExtendedXmlSerializer/)

<img src="https://extendedxmlserializer.github.io/img/logoBig.png" height="200px">

ExtendedXmlSerializer v2 is proudly developed and maintained with ReSharper Ultimate

[<img src="https://blog.dragonspark.us/images/ReSharper.png" height="200px"></a>](https://www.jetbrains.com/resharper/download/)

Welcome!
========

Welcome to ExtendedXMLSerializer's GitHub Repository.  Looking to get started?   Learn about [the basics](https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/wiki/02.-The-Basics).

Got a question or want to submit a new issue?  Make sure you [read our FAQs, first](https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/wiki/01.-FAQs).

For all other cases of curiosity (and otherwise), be sure to check out [our features](https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/wiki/04.-Features) and some [example scenarios](https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/wiki/05.-Example-Scenarios).

(Looking to upgrade from 1.x?  We got you [covered here](https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/wiki/06.-How-to-Upgrade-from-v1.x-to-v2).)

General Information
===================

Support platforms:

- .NET Standard 2.0
- .NET 4.5 ([for now](https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/273))

Feature overview:

-   Deserialization xml from classic `XmlSerializer` (mostly, see: <https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/240>)
-   Serialization class, struct, generic class, primitive type, generic
    list and dictionary, array, enum
-   Serialization class with property interface
-   Serialization circular reference and reference Id
-   Migrate old XML based on an older schema to a current schema.
-   Property encryption
-   Custom serializer
-   Support `XmlElementAttribute`, `XmlRootAttribute`, and `XmlTypeAttribute` for identity resolution
-   Additional attribute support: `XmlIgnoreAttribute`, `XmlAttributeAttribute`, and `XmlEnumAttribute`
-   POCO - all configurations (migrations, custom serializer...) are outside the class and not coupled to attributes or arcane conventions

Standard XML Serializer in .NET is very limited:

-   Does not support serialization of class with circular reference or class with interface property
-   There is no mechanism for reading the old version of XML
-   Does not support properties that are defined with interface types
-   Does not support read-only collection properties (like Xaml does)
-   Does not support parameterized constructors
-   Does not support private constructors
-   If you want create custom serializer, your class must inherit from `IXmlSerializable` or `ISerializable`. 
    This takes the "plain" out of [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object). 😁

Authors
=======

- [Wojciech Nagórski](https://github.com/WojciechNagorski) - v1.x Author.
- [Mike-EEE](https://github.com/Mike-EEE) - v2.x Author.

