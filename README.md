ExtendedXmlSerializer
=====================

[![Build status](https://ci.appveyor.com/api/projects/status/ub776yxp0nj535qp?svg=true)](https://ci.appveyor.com/project/ExtendedXmlSerializer/extendedxmlserializer) [![Nuget](https://img.shields.io/nuget/v/ExtendedXmlSerializer.svg)](https://www.nuget.org/packages/ExtendedXmlSerializer/)

<img src="https://extendedxmlserializer.github.io/img/logoBig.png" height="200px">


Welcome!
========

Welcome to ExtendedXMLSerializer's GitHub repository.  Here you will find a .NET serializer that:

- Specializes in [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object)-based object graph serialization
- Features a powerful extension model
- Operates in the tried-and-trusted dependable XML format. 💖

"But why?"
==========

The classic `System.Xml.XmlSerializer` poses some challenges:

- Does not support properties that are defined with interface types
- Does not support read-only collection properties (like Xaml does)
- Does not support parameterized constructors (immutable objects)
- Does not support private constructors
- Does not support serialization of class with circular reference or class with interface property
- If you want create custom serializer, your class must inherit from `IXmlSerializable` or `ISerializable`.  This takes the "plain" out of [POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object). 😁
- No migration mechanism for XML based on older code model

ExtendedXmlSerializer addresses a lot of these problems and much much more!

- Serializes and deserializes pretty much any POCO you throw at it*: `class`, `struct`, generics, primitives, generic `List` and `Dictionary`, `Array`, `Enum` and much much more! If you find a class that doesn't get serialized, [let us know](https://github.com/ExtendedXmlSerializer/home/issues/new) and we'll take a look at it.
- Custom serializer registration by type or member
- Serialization of references, handling circular references without endlessly looping
- All configurations (migrations, custom serializer...) are outside the class and not coupled to attributes or messy, arcane conventions
- Migrate old XML based on an older schema to a current schema
- Property encryption
- Support `XmlElementAttribute`, `XmlRootAttribute`, and `XmlTypeAttribute` for identity resolution
- Additional attribute support: `XmlIgnoreAttribute`, `XmlAttributeAttribute`, and `XmlEnumAttribute`
- Deserialization xml from classic `XmlSerializer` (mostly, [details in FAQ](https://github.com/ExtendedXmlSerializer/home/wiki/FAQs#systemxmlserializer-vs-extendedxmlserializer))

(\*Yes, this even -- and *especially* -- means classes with properties that have an interface property type!)

Supported platforms:

- .NET Standard 2.0
- .NET 4.5.2 ([as of #273](https://github.com/ExtendedXmlSerializer/home/issues/273))

Usage
=====

ExtendedXmlSerializer uses a `ConfigurationContainer` to store configurations and extensions.  Once this container is configured as desired, call its `Create` method to create a serializer and serialize!

Example class:

``` csharp
class Subject {
    public int Number { get; set; }
    public string Message { get; set; }
}
```

Configure, create, and serialize:

``` csharp
IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
                                                                .UseOptimizedNamespaces()
                                                                .EnableImplicitTyping(typeof(Subject))
                                                                // Additional configurations...
                                                                .Create();

var instance = new Subject {Message = "Hello World!", Number = 42};
var document = serializer.Serialize(new XmlWriterSettings {Indent = true},
                                    instance);
```

MAKE THE PRETTY XML!!! 😁😁😁

(contents of the `document` variable above:)

``` xml
<?xml version="1.0" encoding="utf-8"?>
<Subject Number="42" Message="Hello World!" />
```

The above demonstrated code can be found in the form of [a passing test within our test suite here](https://github.com/ExtendedXmlSerializer/home/blob/a7667b3f56ce15e3146f0ca061e7dae162b1a448/test/ExtendedXmlSerializer.Tests.ReportedIssues/Issue282Tests_README.cs#L11-L33).

Installation
============

From your favorite [Package Manager Console](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell):

```
Install-Package ExtendedXmlSerializer
```

Or if you are brave and want to try out our preview feed:

```
Install-Package ExtendedXmlSerializer -Source https://ci.appveyor.com/nuget/extendedxmlserializer-preview
```

Known Issues
============

While ExtendedXmlSerializer is [very nice](https://tenor.com/view/nice-very-nice-gif-4295060), it does have some known issues that have been identified by the owners as such.  These issues are considered too significant to address and have been consolidated under our label for your review here:

https://github.com/ExtendedXmlSerializer/home/labels/known%20issue

Please review these issues before submitting a new issue and/or trialing ExtendedXmlSerializer.

Featured Documentation
======================

- [FAQs](https://github.com/ExtendedXmlSerializer/home/wiki/FAQs) - Probably the first place to go if you have a question.

- [Conceptual Topics](https://github.com/ExtendedXmlSerializer/home/wiki) - From our wiki.
  - [The Basics](https://github.com/ExtendedXmlSerializer/home/wiki/The-Basics)
  - [Features](https://github.com/ExtendedXmlSerializer/home/wiki/Features#experimental)
  - [API Overview](https://github.com/ExtendedXmlSerializer/home/wiki/API-Overview)
  - [Example Scenarios](https://github.com/ExtendedXmlSerializer/home/wiki/Example-Scenarios)

- [Documentation Site](https://extendedxmlserializer.github.io/documentation/) - A resource deployed on every release.
  - [API Reference](https://extendedxmlserializer.github.io/documentation/reference/)
  - [Concepts and Topics](https://extendedxmlserializer.github.io/documentation/conceptual/) (Mirror of our wiki above)

(Looking to upgrade from 1.x?  We got you [covered here](https://github.com/ExtendedXmlSerializer/home/wiki/How-to-Upgrade-from-v1.x-to-v2).)

Want to Contribute?
===================

We are a smaller project and are open to any contributions or questions.  We do not have a formal code of conduct and would like to keep it that way.

![Keep Calm and Code](https://i.imgur.com/6wP6Zqy.png)

If you view our [FAQs](https://github.com/ExtendedXmlSerializer/home/wiki/FAQs) and still have a question, [open up a new issue](https://github.com/ExtendedXmlSerializer/home/issues/new)!  We'll do our best to [meet you there with sample code](https://github.com/ExtendedXmlSerializer/home/issues?q=is%3Aissue+label%3ADocumentation+sort%3Aupdated-desc) to help get you on your way.

Notable Contributors
========================

- [Wojciech Nagórski](https://github.com/WojciechNagorski) - v1.x Author.
- [Mike-EEE](https://github.com/Mike-EEE) - v2.x Author.

Mentions
========

[<img src="https://github.com/DragonSpark/blog.dragonspark.us/blob/a49ead8aa87f61dd6c4d6f5999e6d01b8823d57b/static/images/ReSharper.png?raw=true" height="200" width="200" />](https://www.jetbrains.com/resharper/download/)

ExtendedXmlSerializer is proudly developed and maintained with ReSharper Ultimate.

<br />
<br />

[<img src="https://raw.githubusercontent.com/DragonSpark/blog.dragonspark.us/a49ead8aa87f61dd6c4d6f5999e6d01b8823d57b/static/images/OzCode.svg?sanitize=true" height="200" />](https://www.oz-code.com/)

Magical debugging is courtesy of OzCode.

<br />
<br />

[<img src="https://raw.githubusercontent.com/SuperDotNet/superdotnet.run/c83812c7120c1d6a1cb0db13bee2d4ff0a5fc526/static/images/CodeCompare.svg?sanitize=true" height="200" />](https://www.devart.com/codecompare/)

File comparison and conflict resolution handled by DevArt's Code Compare.
