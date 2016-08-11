# ExtendedXmlSerializer
Extended Xml Serializer for .Net


## Serialization
```csharp
ExtendedXmlSerializer serializer = new ExtendedXmlSerializer();
var obj = new TestClass();
var xml = serializer.Serialize(obj);
```

## Derialization
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
You must create custom serializer:
```csharp
	public class TestClassSerializer : AbstractCustomSerializator<TestClass>
    {
        public override TestClass Read(XElement element)
        {
            return new TestClass(element.Element("String").Value);
        }

        public override void Write(XmlWriter writer, TestClass obj)
        {
            writer.WriteElementString("String", obj.PropStr);
        }
    }
```
Then you must register your custom serializer. See point configuration.

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

You can migrate old version of xml:
```csharp
    public class TestClassMigrationMap : AbstractMigrationMap<TestClass>
    {
        private static readonly Dictionary<int, Action<XElement>> MigrationMap = new Dictionary<int, Action<XElement>>
            {
                {0, MigrationV0 },
                {1, MigrationV1 }
            };

        public override int Version
        {
            get { return 2; }
        }

        public override Dictionary<int, Action<XElement>> Migrations
        {
            get { return MigrationMap; }
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
Then you must register your MigrationMap. See point configuration.

## Configuration
For use MigrationMap and CustomSerializator you must register them in ExtendedXmlSerializer. You can do this in two ways.

#### Use SimpleSerializationToolsFactory class
```csharp
var toolsFactory = new SimpleSerializationToolsFactory();
// Register custom serializer
toolsFactory.CustomSerializators.Add(new TestClassSerializer());
// Register MigrationMap
toolsFactory.MigrationMaps.Add(new TestClassMigrationMap());
ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);
```

#### Use Autofac integration
```csharp
var builder = new ContainerBuilder();
// Register ExtendedXmlSerializer module
builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
// Register custom serializer
builder.RegisterType<TestClassSerializer>().As<ICustomSerializator<TestClass>>().SingleInstance();
// Register MigrationMap
builder.RegisterType<TestClassMigrationMap>().As<IMigrationMap<TestClass>>().SingleInstance();
var containter = builder.Build();

// Resolve ExtendedXmlSerializer
var serializer = containter.Resolve<IExtendedXmlSerializer>();
```


