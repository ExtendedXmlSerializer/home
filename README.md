[![Build status](https://ci.appveyor.com/api/projects/status/9u1w8cyyr22kbcwi?svg=true)](https://ci.appveyor.com/project/wojtpl2/extendedxmlserializer) [![NuGet](https://img.shields.io/nuget/v/ExtendedXmlSerializer.svg)](https://www.nuget.org/packages/ExtendedXmlSerializer/)
# ExtendedXmlSerializer
Extended Xml Serializer for .NET

Support platforms
* .NET 4.5 
* .NET Platform Standard 1.6

Support features
* deserialization xml from standard XMLSerializer
* serialization class, struct, generic class, primitive type, generic list, array, enum
* serialization class with property interface
* serialization circular reference and reference Id
* deserialization old version of xml
* custom serializer
* POCO - all configurations (migrations, custom serializer...) is outside the class

## Serialization
```csharp
ExtendedXmlSerializer serializer = new ExtendedXmlSerializer();
var obj = new TestClass();
var xml = serializer.Serialize(obj);
```

## Deserialization
```csharp
var obj2 = serializer.Deserialize<TestClass>(xml);
```

## Custom serialization
If your class has to be serialized in a non-standard way:
```csharp
    public class TestClass
    {
        public TestClass(string paramStr)
        {
            PropStr = paramStr;
        }

        public string PropStr { get; private set; }
    }
```
You must configure custom serializer:
```csharp
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
```
Then you must register your TestClassConfig class. See point configuration.

## Deserialize old version of xml
If you had a class:
```csharp
    public class TestClass
    {
        public int Id { get; set; }
        public string Type { get; set; } 
    }
```
Then you renamed property:
```csharp
    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } 
    }
```
Then you added new property and you want to calculate a new value while deserialization.
```csharp
    public class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Value { get; set; }
    }
```

You can migrate old version of xml using migrations:
```csharp
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
```
Then you must register your TestClassConfig class. See point configuration.

## Object reference and circular reference
If you have a class:
```csharp
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
```

then you create object with circular reference, like this:
```csharp
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
```

You must configure Person class as reference object:
```csharp
    public class PersonConfig : ExtendedXmlSerializerConfig<Person>
    {
        public PersonConfig()
        {
            ObjectReference(p => p.Id);
        }
    }
```
Then you must register your PersonConfig class. See point configuration.

Output xml will look like this:
```xml
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
```

## Configuration
For use config class you must register them in ExtendedXmlSerializer. You can do this in two ways.

#### Use SimpleSerializationToolsFactory class
```csharp
var toolsFactory = new SimpleSerializationToolsFactory();
// Register your config class
toolsFactory.Configurations.Add(new TestClassConfig());

ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);
```

#### Use Autofac integration
```csharp
var builder = new ContainerBuilder();
// Register ExtendedXmlSerializer module
builder.RegisterModule<AutofacExtendedXmlSerializerModule>();

// Register your config class
builder.RegisterType<TestClassConfig>().As<ExtendedXmlSerializerConfig<TestClass>>().SingleInstance();

var containter = builder.Build();

// Resolve ExtendedXmlSerializer
var serializer = containter.Resolve<IExtendedXmlSerializer>();
```


