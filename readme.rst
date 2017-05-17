.. image:: https://img.shields.io/nuget/v/ExtendedXmlSerializer.svg   :target: https://www.nuget.org/packages/ExtendedXmlSerializer/
.. image:: https://ci.appveyor.com/api/projects/status/9u1w8cyyr22kbcwi?svg=true   :target: https://ci.appveyor.com/project/wojtpl2/extendedxmlserializer


Information
===========

Support platforms:

* .NET 4.5
* .NET Platform Standard 1.6

Support features:

* Deserialization xml from standard XMLSerializer
* Serialization class, struct, generic class, primitive type, generic list and dictionary, array, enum
* Serialization class with property interface
* Serialization circular reference and reference Id
* Deserialization of old version of xml
* Property encryption
* Custom serializer
* Support XmlElementAttribute and XmlRootAttribute
* POCO - all configurations (migrations, custom serializer...) are outside the clas

Standard XML Serializer in .NET is very limited:

* Does not support serialization of class with circular reference or class with interface property.
* There is no mechanism for reading the old version of XML.
* If you want create custom serializer, your class must inherit from IXmlSerializable. This means that your class will not be a POCO class.
* Does not support IoC

Serialization
=============


.. sourcecode:: csharp

    var serializer = new ConfigurationContainer().Create();
    var obj = new TestClass();
    var xml = serializer.Serialize(obj);

Deserialization
===============


.. sourcecode:: csharp

    var obj2 = serializer.Deserialize<TestClass>(xml);

Serialization of dictionary
===========================

You can serialize generic dictionary, that can store any type.

.. sourcecode:: csharp

    public class TestClass
    {
        public Dictionary<int, string> Dictionary { get; set; }
    }


.. sourcecode:: csharp

    var obj = new TestClass
    {
        Dictionary = new Dictionary<int, string>
        {
            {1, "First"},
            {2, "Second"},
            {3, "Other"},
        }
    };

Output XML will look like:

.. sourcecode:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <TestClass xmlns="clr-namespace:ExtendedXmlSerialization.Samples.Dictianary;assembly=ExtendedXmlSerializer.Samples">
      <Dictionary>
        <Item xmlns="https://extendedxmlserializer.github.io/system">
          <Key>1</Key>
          <Value>First</Value>
        </Item>
        <Item xmlns="https://extendedxmlserializer.github.io/system">
          <Key>2</Key>
          <Value>Second</Value>
        </Item>
        <Item xmlns="https://extendedxmlserializer.github.io/system">
          <Key>3</Key>
          <Value>Other</Value>
        </Item>
      </Dictionary>
    </TestClass>

If you use UseOptimizedNamespaces function xml will look like:

.. sourcecode:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <TestClass xmlns:sys="https://extendedxmlserializer.github.io/system" xmlns:exs="https://extendedxmlserializer.github.io/v2" xmlns="clr-namespace:ExtendedXmlSerialization.Samples.Dictianary;assembly=ExtendedXmlSerializer.Samples">
      <Dictionary>
        <sys:Item>
          <Key>1</Key>
          <Value>First</Value>
        </sys:Item>
        <sys:Item>
          <Key>2</Key>
          <Value>Second</Value>
        </sys:Item>
        <sys:Item>
          <Key>3</Key>
          <Value>Other</Value>
        </sys:Item>
      </Dictionary>
    </TestClass>

Custom serialization
====================

If your class has to be serialized in a non-standard way:

.. sourcecode:: csharp

        public class TestClass
        {
            public TestClass(string paramStr, int paramInt)
            {
                PropStr = paramStr;
                PropInt = paramInt;
            }
    
            public string PropStr { get; private set; }
            public int PropInt { get; private set; }
        }

You must create custom serializer:

.. sourcecode:: csharp

        public class TestClassSerializer : IExtendedXmlCustomSerializer<TestClass>
        {
            public TestClass Deserialize(XElement element)
            {
                var xElement = element.Member("String");
                var xElement1 = element.Member("Int");
                if (xElement != null && xElement1 != null)
                {
                    var strValue = xElement.Value;
    
                    var intValue = Convert.ToInt32(xElement1.Value);
                    return new TestClass(strValue, intValue);
                }
                throw new InvalidOperationException("Invalid xml for class TestClassWithSerializer");
            }
    
            public void Serializer(XmlWriter writer, TestClass obj)
            {
                writer.WriteElementString("String", obj.PropStr);
                writer.WriteElementString("Int", obj.PropInt.ToString(CultureInfo.InvariantCulture));
            }
        }

Then, you have to add custom serializer to configuration of TestClass:

.. sourcecode:: csharp

    var serializer = new ConfigurationContainer().Type<TestClass>()
        .CustomSerializer(new TestClassSerializer())
        .Configuration
        .Create();

Deserialize old version of xml
==============================

In standard XMLSerializer you can't deserialize XML in case you change model. In ExtendedXMLSerializer you can create migrator for each class separately. E.g.: If you have big class, that uses small class and this small class will be changed you can create migrator only for this small class. You don't have to modify whole big XML. Now I will show you a simple example:
If you had a class:

.. sourcecode:: csharp

    public class TestClass
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

and generated XML look like:

.. sourcecode:: xml

    <? xml version="1.0" encoding="utf-8"?>
    <TestClass xmlns="clr-namespace:ExtendedXmlSerialization.Samples.MigrationMap;assembly=ExtendedXmlSerializer.Samples">
      <Id>1</Id>
      <Type>Type</Type>
    </TestClass>

Then you renamed property:

.. sourcecode:: csharp

    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

and generated XML look like:

.. sourcecode:: xml

    <? xml version="1.0" encoding="utf-8"?>
    <TestClass xmlns:exs="https://extendedxmlserializer.github.io/v2" exs:version="1" xmlns="clr-namespace:ExtendedXmlSerialization.Samples.MigrationMap;assembly=ExtendedXmlSerializer.Samples">
      <Id>1</Id>
      <Name>Type</Name>
    </TestClass>

Then, you added new property and you wanted to calculate a new value during deserialization.

.. sourcecode:: csharp

    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

and new XML should look like:

.. sourcecode:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <TestClass xmlns:exs="https://extendedxmlserializer.github.io/v2" exs:version="2" xmlns="clr-namespace:ExtendedXmlSerialization.Samples.MigrationMap;assembly=ExtendedXmlSerializer.Samples">
      <Id>1</Id>
      <Name>Type</Name>
      <Value>Calculated</Value>
    </TestClass>

You can migrate (read) old version of XML using migrations:

.. sourcecode:: csharp

        public class TestClassMigrations : IEnumerable<Action<XElement>>
        {
            public static void MigrationV0(XElement node)
            {
                var typeElement = node.Member("Type");
                // Add new node
                node.Add(new XElement("Name", typeElement.Value));
                // Remove old node
                typeElement.Remove();
            }
    
            public static void MigrationV1(XElement node)
            {
                // Add new node
                node.Add(new XElement("Value", "Calculated"));
            }
    
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
            public IEnumerator<Action<XElement>> GetEnumerator()
            {
                yield return MigrationV0;
                yield return MigrationV1;
            }
        }

Then, you must register your TestClassMigrations class in configuration

.. sourcecode:: csharp

    var serializer = new ConfigurationContainer().ConfigureType<TestClass>()
                                                .AddMigration(new TestClassMigrations())
                                                .Configuration
                                                .Create();

Object reference and circular reference
=======================================

If you have a class:

.. sourcecode:: csharp

        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
    
            public Person Boss { get; set; }
        }
    
        public class Company
        {
            public List<Person> Employees { get; set; }
        }

then you create object with circular reference, like this:

.. sourcecode:: csharp

    var boss = new Person {Id = 1, Name = "John"};
    boss.Boss = boss; //himself boss
    var worker = new Person {Id = 2, Name = "Oliver"};
    worker.Boss = boss;
    var obj = new Company
    {
        Employees = new List<Person>
        {
            worker,
            boss
        }
    };

You must configure Person class as reference object:

.. sourcecode:: csharp

    var serializer = new ConfigurationContainer().ConfigureType<Person>()
                                                .EnableReferences(p => p.Id)
                                                .Configuration
                                                .Create();

Output XML will look like this:

.. sourcecode:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <Company xmlns="clr-namespace:ExtendedXmlSerialization.Samples.ObjectReference;assembly=ExtendedXmlSerializer.Samples">
      <Employees>
        <Capacity>4</Capacity>
        <Person Id="2">
          <Name>Oliver</Name>
          <Boss Id="1">
            <Name>John</Name>
            <Boss xmlns:exs="https://extendedxmlserializer.github.io/v2" exs:entity="1" />
          </Boss>
        </Person>
        <Person xmlns:exs="https://extendedxmlserializer.github.io/v2" exs:entity="1" />
      </Employees>
    </Company>

Property Encryption
===================

If you have a class with a property that needs to be encrypted:

.. sourcecode:: csharp

    public class Person
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

You must implement interface IEncryption. For example, it will show the Base64 encoding, but in the real world better to use something safer, eg. RSA.:

.. sourcecode:: csharp

        public class CustomEncryption : ConverterBase<string>, IEncryption
        {
            public override string Parse(string data)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(data));
            }
    
            public override string Format(string instance)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(instance));
            }
        }

Then, you have to specify which properties are to be encrypted and register your IEncryption implementation.

.. sourcecode:: csharp

    var serializer = new ConfigurationContainer()
        .UseEncryptionAlgorithm(new CustomEncryption())
        .ConfigureType<Person>().Member(p => p.Password).Encrypt()
        .Configuration
        .Create();

History
=======


* 2017-??-?? - v2.0.0 - Rewritten version

Authors
=======


* `Wojciech Nagórski <https://github.com/wojtpl2>`__
* `Mike-EEE <https://github.com/Mike-EEE>`__

