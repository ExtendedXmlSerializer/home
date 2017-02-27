Information
=============

Support platforms 

* .NET 4.5
* .NET Platform Standard 1.6

Support framework:

- ASP.NET Core
- WebApi

Support features:

# Deserialization xml from standard XMLSerializer
# Serialization class, struct, generic class, primitive type, generic list and dictionary, array, enum
# Serialization class with property interface
# Serialization circular reference and reference Id
# Deserialization of old version of xml
# Property encryption
# Custom serializer
# Support XmlElementAttribute and XmlRootAttribute
# POCO - all configurations (migrations, custom serializer...) are outside the clas

Standard XML Serializer in .NET is very limited. 

- Does not support serialization of class with circular reference or class with interface property.
- There is no mechanism for reading the old version of XML.
- If you want create custom serializer, your class must inherit from IXmlSerializable. This means that your class will not be a POCO class.
- Does not support IoC

Serialization
=============

.. code:: csharp

    ExtendedXmlSerializer serializer = new ExtendedXmlSerializer();
    var obj = new TestClass();
    var xml = serializer.Serialize(obj);

Deserialization
===============

.. code:: ccsharp

    var obj2 = serializer.Deserialize<TestClass>(xml);

Serialization of dictionary
===========================

You can serialize generic dictionary, that can store any type.

.. code:: csharp

    public class TestClass
    {
        public Dictionary<int, string> Dictionary { get; set; }
    }
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

.. code:: xml

    <TestClass type="Samples.TestClass">
      <Dictionary>
        <Item>
            <Key>1</Key>
            <Value>First</Value>
        </Item>
        <Item>
            <Key>2</Key>
            <Value>Second</Value>
        </Item>
        <Item>
            <Key>3</Key>
            <Value>Other</Value>
        </Item>
      </Dictionary>
    </TestClass>

Custom serialization
====================

If your class has to be serialized in a non-standard way:

.. code:: csharp

        public class TestClass
        {
            public TestClass(string paramStr)
            {
                PropStr = paramStr;
            }

            public string PropStr { get; private set; }
        }

You must configure custom serializer:

.. code:: csharp

        public class TestClassConfig : ExtendedXmlSerializerConfig<TestClass>
        {
            public TestClassConfig()
            {
                CustomSerializer(Serializer, Deserialize);
            }

            public TestClass Deserialize(XElement element)
            {
                return new TestClass(element.Element("String").Value);
            }

            public void Serializer(XmlWriter writer, TestClass obj)
            {
                writer.WriteElementString("String", obj.PropStr);
            }
        }

Then, you must register your TestClassConfig class. See point
configuration.

Deserialize old version of xml
==============================

In standard XMLSerializer you can't deserialize XML in case you change
model. In ExtendedXMLSerializer you can create migrator for each class
separately. E.g.: If you have big class, that uses small class and this
small class will be changed you can create migrator only for this small
class. You don't have to modify whole big XML. Now I will show you a
simple example:

If you had a class:

.. code:: csharp

        public class TestClass
        {
            public int Id { get; set; }
            public string Type { get; set; } 
        }

and generated XML look like:

.. code:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <TestClass type="Samples.TestClass">
      <Id>1</Id>
      <Type>Type</Type>
    </TestClass>

Then you renamed property:

.. code:: csharp

        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } 
        }

and generated XML look like:

.. code:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <TestClass type="Samples.TestClass" ver="1">
      <Id>1</Id>
      <Name>Type</Name>
    </TestClass>

Then, you added new property and you wanted to calculate a new value
during deserialization.

.. code:: csharp

        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } 
            public string Value { get; set; }
        }

and new XML should look like:

.. code:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <TestClass type="Samples.TestClass" ver="2">
      <Id>1</Id>
      <Name>Type</Name>
      <Value>Calculated</Value>
    </TestClass>

You can migrate (read) old version of XML using migrations:

.. code:: csharp

        public class TestClassConfig : ExtendedXmlSerializerConfig<TestClass>
        {
            public TestClassConfig()
            {
                AddMigration(MigrationV0).AddMigration(MigrationV1);
            }

            public static void MigrationV0(XElement node)
            {
                var typeElement = node.Elements().FirstOrDefault(x => x.Name == "Type");
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
        }

Then, you must register your TestClassConfig class. See point
configuration.

Object reference and circular reference
=======================================

If you have a class:

.. code:: csharp

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

.. code:: csharp

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

.. code:: csharp

        public class PersonConfig : ExtendedXmlSerializerConfig<Person>
        {
            public PersonConfig()
            {
                ObjectReference(p => p.Id);
            }
        }

Then, you must register your PersonConfig class. See point
configuration.

Output XML will look like this:

.. code:: xml

    <?xml version="1.0" encoding="UTF-8"?>
    <Company type="Samples.Company">
       <Employees>
          <Person type="Samples.Person" id="2">
             <Id>2</Id>
             <Name>Oliver</Name>
             <Boss type="Samples.Person" ref="1" />
          </Person>
          <Person type="Samples.Person" id="1">
             <Id>1</Id>
             <Name>John</Name>
             <Boss type="Samples.Person" ref="1" />
          </Person>
       </Employees>
    </Company>

Property Encryption
===================

If you have a class with a property that needs to be encrypted:

.. code:: csharp

        public class Person
        {
            public string Name { get; set; }
            public string Password { get; set; }
        }

You must implement interface IPropertyEncryption. For example, it will
show the Base64 encoding, but in the real world better to use something
safer, eg. RSA.:

.. code:: csharp

        public class Base64PropertyEncryption : IPropertyEncryption
        {
            public string Encrypt(string value)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }

            public string Decrypt(string value)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
        }

In the Person class configuration you need to specify which properties
are to be encrypted:

.. code:: csharp

        public class PersonConfig : ExtendedXmlSerializerConfig<Person>
        {
            public PersonConfig()
            {
                Encrypt(p => p.Password);
            }
        }

Then, you must register your PersonConfig class and your implementation
of IPropertyEncryption. See point configuration.

Configuration
=============

For using config class, you must register them in ExtendedXmlSerializer.
You can do this in two ways.

Use SimpleSerializationToolsFactory class
-----------------------------------------

.. code:: csharp

    var toolsFactory = new SimpleSerializationToolsFactory();

    // Register your config class
    toolsFactory.Configurations.Add(new TestClassConfig());

    // If you want to use property encryption you must register your implementation of IPropertyEncryption, e.g.:
    toolsFactory.EncryptionAlgorithm = new Base64PropertyEncryption(); 

    ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);

Use Autofac integration
-----------------------

.. code:: csharp

    var builder = new ContainerBuilder();
    // Register ExtendedXmlSerializer module
    builder.RegisterModule<AutofacExtendedXmlSerializerModule>();

    // Register your config class
    builder.RegisterType<TestClassConfig>().As<ExtendedXmlSerializerConfig<TestClass>>().SingleInstance();

    // If you want to use property encryption you must register your implementation of IPropertyEncryption, e.g.:
    builder.RegisterType<Base64PropertyEncryption>().As<IPropertyEncryption>().SingleInstance();

    var containter = builder.Build();

    // Resolve ExtendedXmlSerializer
    var serializer = containter.Resolve<IExtendedXmlSerializer>();

ASP.NET Core integration
========================

You can integrate the ExtendedXmlSerializer with ASP.NET Core, so that
your services will generate XML using a ExtendedXmlSerializer. You only
need to install
`ExtendedXmlSerializer.AspCore <https://www.nuget.org/packages/ExtendedXmlSerializer.AspCore/>`__
and configure it in Startup.cs.

Use SimpleSerializationToolsFactory class
-----------------------------------------

This configuration is very simple. You just need create configuration
for ExtendedXmlSerializer and add formatters to MVC.

.. code:: csharp

    public void ConfigureServices(IServiceCollection services)
    {
        // Custom create ExtendedXmlSerializer
        SimpleSerializationToolsFactory factory = new SimpleSerializationToolsFactory();
        factory.Configurations.Add(new TestClassConfig());
        IExtendedXmlSerializer serializer = new ExtendedXmlSerializer(factory);

        // Add services to the collection.
        services.AddMvc(options =>
        {
            options.RespectBrowserAcceptHeader = true; // false by default

            //Add ExtendedXmlSerializer's formatter
            options.OutputFormatters.Add(new ExtendedXmlSerializerOutputFormatter(serializer));
            options.InputFormatters.Add(new ExtendedXmlSerializerInputFormatter(serializer));
        });
    }

Use Autofac integration
-----------------------

This configuration is more difficult but recommended. You have to
install
`Autofac.Extensions.DependencyInjectio <www.nuget.org/packages/Autofac.Extensions.DependencyInjection/>`__
and read Autofac
`documentation <docs.autofac.org/en/latest/integration/aspnetcore.html>`__.
The following code adds an MVC service and creates a container AutoFac.

.. code:: csharp

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        // Add services to the collection.
        services.AddMvc(options =>
        {
            options.RespectBrowserAcceptHeader = true; // false by default

            //Resolve ExtendedXmlSerializer
            IExtendedXmlSerializer serializer = ApplicationContainer.Resolve<IExtendedXmlSerializer>();

            //Add ExtendedXmlSerializer's formatter
            options.OutputFormatters.Add(new ExtendedXmlSerializerOutputFormatter(serializer));
            options.InputFormatters.Add(new ExtendedXmlSerializerInputFormatter(serializer));
        });

        // Create the container builder.
        var builder = new ContainerBuilder();

        // Register dependencies, populate the services from
        // the collection, and build the container. If you want
        // to dispose of the container at the end of the app,
        // be sure to keep a reference to it as a property or field.
        builder.Populate(services);
        builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
        builder.RegisterType<TestClassConfig>().As<ExtendedXmlSerializerConfig<TestClass>>().SingleInstance();
        this.ApplicationContainer = builder.Build();

        // Create the IServiceProvider based on the container.
        return new AutofacServiceProvider(this.ApplicationContainer);
    }

In this case, you can also inject IExtendedXmlSerializer into your
controller:

.. code:: csharp

        [Route("api/[controller]")]
        public class TestClassController : Controller
        {
            private readonly IExtendedXmlSerializer _serializer;

            public TestClassController(IExtendedXmlSerializer serializer)
            {
                _serializer = serializer;
            }

            ...
        } 

WebApi integration
==================

You can integrate ExtendedXmlSerializer with WebApi, so that your
services will generate XML using a ExtendedXmlSerializer. You only need
to install
`ExtendedXmlSerializer.WebApi <www.nuget.org/packages/ExtendedXmlSerializer.WebApi/>`__
and configure it in WebApi configuration. You can do it using autofac or
SimpleSerializationToolsFactory e.g.:

.. code:: csharp

    public static void Register(HttpConfiguration config)
    {
        // Manual creation of IExtendedXmlSerializer or resolve it from AutoFac.
        var simpleConfig = new SimpleSerializationToolsFactory();
        simpleConfig.Configurations.Add(new TestClassConfig());
        var serializer = new ExtendedXmlSerializer(simpleConfig);

        config.RegisterExtendedXmlSerializer(serializer);

        // Web API routes
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );
    }

History
=======

-  2016-12-06 - v1.5.0 - Support property without set accessor for
   collections. Support IDictionary, IList and ISet properties.
   Attribute type is added only when necessary.
-  2016-11-18 - v1.4.1 - Support sorting parameters and fix
   serialization of Object property
-  2016-11-15 - v1.4.0 - Support XmlElementAttribute and
   XmlRootAttribute
-  2016-10-18 - v1.3.0 - Property encryption
-  2016-09-22 - v1.2.0 - Support of Dictionary
-  2016-09-14 - v1.1.0 - Integration with ASP.NET Core and WebApi
-  2016-08-31 - v1.0.0 - First version

Contributors
============

-  `Wojciech Nagórski <https://github.com/wojtpl2>`__ - original version
-  `Mike-EEE <https://github.com/Mike-EEE>`__ - many improvements

.. |Build status| image:: https://ci.appveyor.com/api/projects/status/9u1w8cyyr22kbcwi?svg=true
   :target: https://ci.appveyor.com/project/wojtpl2/extendedxmlserializer
.. |NuGet| image:: https://img.shields.io/nuget/v/ExtendedXmlSerializer.svg
   :target: https://www.nuget.org/packages/ExtendedXmlSerializer/
