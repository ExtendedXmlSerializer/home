using System;
using System.IO;

namespace ExtendedXmlSerializer.DocGenerator
{
	class Program
	{
		// ReSharper disable once UnusedParameter.Local
		static void Main(string[] args)
		{
			ReStructuredText doc = new ReStructuredText();

			doc.AddHeader("Information");

			doc.Add("Support platforms:");
			doc.AddList(".NET 4.5", ".NET Platform Standard 1.6");
			doc.Add("Support features:");
			doc.AddList(
				"Deserialization xml from standard XMLSerializer",
				"Serialization class, struct, generic class, primitive type, generic list and dictionary, array, enum",
				"Serialization class with property interface",
				"Serialization circular reference and reference Id",
				"Deserialization of old version of xml",
				"Property encryption",
				"Custom serializer",
				"Support XmlElementAttribute and XmlRootAttribute",
				"POCO - all configurations (migrations, custom serializer...) are outside the clas");

			doc.Add("Standard XML Serializer in .NET is very limited:");
			doc.AddList(
				"Does not support serialization of class with circular reference or class with interface property.",
				"There is no mechanism for reading the old version of XML.",
				"Does not support properties that are defined with interface types.",
				"Does not support read-only collection properties (like Xaml does).",
				"Does not support parameterized constructors.",
				"Does not support private constructors.",
				"If you want create custom serializer, your class must inherit from IXmlSerializable. This means that your class will not be a POCO class.",
				"Does not support IoC");

			doc.AddHeader("The Basics");
			doc.Add("Everything in ExtendedXmlSerializer begins with a configuration container, from which you can use to configure the serializer and ultimately create it:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Introduction\Create.cs", "Create");

			doc.Add("Using this simple subject class:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Introduction\Subject.cs", "Subject");

			doc.Add("The results of the default serializer will look like this:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Introduction.xml", CodeFormat.Xml);

			doc.Add("We can take this a step further by configuring the `Subject`'s Type and Member properties, which will effect how its Xml is emitted.  Here is an example of configuring the `Subject`'s name to emit as `ModifiedSubject`:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Introduction\Type.cs", "Type");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Introduction.Type.xml", CodeFormat.Xml);

			doc.Add("Diving a bit further, we can also configure the type's member information.  For example, configuring `Subject.Message` to emit as `Text` instead:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Introduction\Member.cs", "Member");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Introduction.Member.xml", CodeFormat.Xml);

			doc.AddHeader("Xml Settings");
			doc.Add("In case you want to configure the XML write and read settings via `XmlWriterSettings` and `XmlReaderSettings` respectively, you can do that via extension methods created for you to do so:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Introduction\Settings.cs", "Write");

			doc.Add("And for reading:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Introduction\Settings.cs", "Read");

			doc.AddHeader("Serialization");
			doc.Add("Now that your configuration container has been configured and your serializer has been created, it's time to get to the serialization.");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Simple\\SimpleSamples.cs",
				"Serialization");

			doc.AddHeader("Deserialization");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Simple\\SimpleSamples.cs",
				"Deserialization");

			doc.AddHeader("Fluent API");
			doc.Add("ExtendedXmlSerializer use fluent API to configuration. Example");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\FluentApi\\FluentApiSamples.cs", "FluentAPI");


			doc.AddHeader("Serialization of dictionary");
			doc.Add("You can serialize generic dictionary, that can store any type.");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Dictianary\\TestClass.cs", "TestClass");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Dictianary\\DictianarySamples.cs", "InitDictionary");
			doc.Add("Output XML will look like:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\bin\\DictianarySamples.xml", CodeFormat.Xml);
			doc.Add("If you use UseOptimizedNamespaces function xml will look like:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\bin\\DictianarySamplesUseOptimizedNamespaces.xml", CodeFormat.Xml);

			doc.AddHeader("Custom serialization");
			doc.Add("If your class has to be serialized in a non-standard way:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\CustomSerializator\\TestClass.cs", "CustomSerializator");
			doc.Add("You must create custom serializer:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\CustomSerializator\\TestClassSerializer.cs", "TestClassSerializer");
			doc.Add("Then, you have to add custom serializer to configuration of TestClass:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\CustomSerializator\\CustomSerializatorSamples.cs", "AddCustomSerializerToConfiguration");

			doc.AddHeader("Deserialize old version of xml");
			doc.Add("In standard XMLSerializer you can't deserialize XML in case you change model. In ExtendedXMLSerializer you can create migrator for each class separately. E.g.: If you have big class, that uses small class and this small class will be changed you can create migrator only for this small class. You don't have to modify whole big XML. Now I will show you a simple example:");
			doc.Add("If you had a class:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\TestClass.cs", "FirstVersion");
			doc.Add("and generated XML look like:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\TestClass.cs", "XmlFirstVersion", CodeFormat.Xml);
			doc.Add("Then you renamed property:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\TestClass.cs", "SecondVersion");
			doc.Add("and generated XML look like:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\TestClass.cs", "XmlSecondVersion", CodeFormat.Xml);
			doc.Add("Then, you added new property and you wanted to calculate a new value during deserialization.");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\TestClass.cs", "LastVersion");
			doc.Add("and new XML should look like:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\bin\\XmlLastVersion.xml", CodeFormat.Xml);
			doc.Add("You can migrate (read) old version of XML using migrations:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\TestClassMigrations.cs", "TestClassMigrations");
			doc.Add("Then, you must register your TestClassMigrations class in configuration");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\MigrationMap\\MigrationMapSamples.cs", "MigrationsConfiguration");

			doc.AddHeader("Extensibility");
			doc.Add("With type and member configuration out of the way, we can turn our attention to what really makes ExtendedXmlSeralizer tick: extensibility.  As its name suggests, ExtendedXmlSeralizer offers a very flexible (but albeit new) extension model from which you can build your own extensions.  Pretty much all if not all features you encounter with ExtendedXmlSeralizer are through extensions.  There are quite a few in our latest version here that showcase this extensibility.  The remainder of this document will showcase the top features of ExtendedXmlSerializer that are accomplished through its extension system.");
			doc.Add(string.Empty);

			doc.AddHeader("Object reference and circular reference");
			doc.Add("If you have a class:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\ObjectReference\\Person.cs", "PersonClass");
			doc.Add("then you create object with circular reference, like this:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\ObjectReference\\ObjectReferenceSamples.cs", "CreateObject");
			doc.Add("You must configure Person class as reference object:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\ObjectReference\\ObjectReferenceSamples.cs", "Configure");
			doc.Add("Output XML will look like this:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\bin\\ObjectReferenceSamples.xml", CodeFormat.Xml);

			doc.AddHeader("Property Encryption");
			doc.Add("If you have a class with a property that needs to be encrypted:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Encrypt\\Person.cs", "EncryptClass");
			doc.Add("You must implement interface IEncryption. For example, it will show the Base64 encoding, but in the real world better to use something safer, eg. RSA.:");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Encrypt\\EncryptSamples.cs", "CustomEncryption");
			doc.Add("Then, you have to specify which properties are to be encrypted and register your IEncryption implementation.");
			doc.AddCode("..\\..\\..\\..\\samples\\ExtendedXmlSerializer.Samples\\Encrypt\\EncryptSamples.cs", "Configuration");

			doc.AddHeader("Custom Conversion");
			doc.Add("ExtendedXmlSerializer does a pretty decent job (if we do say so ourselves) of composing and decomposing objects, but if you happen to have a type that you want serialized in a certain way, and this type can be destructured into a `string`, then you can register a custom converter for it.");
			doc.Add(string.Empty);

			doc.Add("Using the following:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\CustomStructConverter.cs", "CustomConverter");

			doc.Add("Register the converter:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\Converters.cs", "Converter");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.Converters.xml", CodeFormat.Xml);

			doc.AddHeader("Optimized Namespaces");
			doc.Add(@"By default Xml namespaces are emitted on an ""as needed"" basis:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.OptimizedNamepsaces.Default.xml", CodeFormat.Xml);

			doc.Add(@"But with one call to the `UseOptimizedNamespaces` call, namespaces get placed at the root of the document, thereby reducing document footprint:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\OptimizedNamespaces.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.OptimizedNamepsaces.Optimized.xml", CodeFormat.Xml);

			doc.AddHeader("Implicit Namespaces/Typing");
			doc.Add(@"If you don't like namespaces at all, you can register types so that they do not emit namespaces when they are rendered into a document:");

			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\ImplicitTypes.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.ImplicitTypes.xml", CodeFormat.Xml);

			doc.AddHeader("Auto-Formatting (Attributes)");
			doc.Add("The default behavior for emitting data in an Xml document is to use elements, which can be a little chatty and verbose:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\OptimizedNamespaces.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.AutoFormatting.Default.xml", CodeFormat.Xml);

			doc.Add("Making use of the `UseAutoFormatting` call will enable all types that have a registered `IConverter` (convert to string and back) to emit as attributes:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.AutoFormatting.Enabled.xml", CodeFormat.Xml);

			doc.AddHeader("Private Constructors");
			doc.Add("One of the limitations of the classic XmlSerializer is that it does not support private constructors, but ExtendedXmlSerializer does via its `EnableAllConstructors` call:");

			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\PrivateConstructors.cs", "Subject");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\PrivateConstructors.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.PrivateConstructors.xml", CodeFormat.Xml);

			doc.AddHeader("Parameterized Members and Content");
			doc.Add("Taking this concept bit further leads to a favorite feature of ours in ExtendedXmlSerlializer.  The classic serializer only supports parameterless public constructors.  With ExtendedXmlSerializer, you can use the `EnableParameterizedContent` call to enable parameterized parameters in the constructor that by convention have the same name as the property for which they are meant to assign:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\ParameterizedContent.cs", "Subject");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\ParameterizedContent.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.ParameterizedContent.xml", CodeFormat.Xml);

			doc.AddHeader("Tuples");
			doc.Add("By enabling parameterized content, it opens up a lot of possibilities, like being able to serialize Tuples.  Of course, serializable Tuples were introduced recently with the latest version of C#.  Here, however, you can couple this with our member-naming funtionality and provide better naming for your tuple properties:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\Tuples.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.Tuples.xml", CodeFormat.Xml);

			doc.AddHeader("Experimental Xaml-ness: Attached Properties");
			doc.Add("We went ahead and got a little cute with v2 of ExtendedXmlSerializer, adding support for Attached Properties on objects in your serialized object graph.  But instead of constraining it to objects that inherit from `DependencyObject`, *every* object can benefit from it.  Check it out:");

			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\AttachedProperties.cs", "Properties");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\AttachedProperties.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Extensibility.AttachedProperties.xml", CodeFormat.Xml);

			doc.Add("(Please note that this feature is experimental, but please try it out and let us know what you think!)");
			doc.Add(string.Empty);

			doc.AddHeader("Experimental Xaml-ness: Markup Extensions");
			doc.Add("Saving the best feaure for last, we have experimental support for one of Xaml's greatest features, Markup Extensions:");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\MarkupExtensions.cs", "Extension");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Extensibility\MarkupExtensions.cs", "Example");

			doc.Add("(Please note that this feature is experimental, but please try it out and let us know what you think!)");
			doc.Add(string.Empty);

			doc.AddHeader("How to Upgrade from v1.x to v2");
			doc.Add("Finally, if you have documents from v1, you will need to upgrade them to v2 to work.  This involves reading the document in an instance of v1 serializer, and then writing it in an instance of v2 serializer.  We have provided the `ExtendedXmlSerializer.Legacy` nuget package to assist in this goal.");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Upgrade.Example.v1.xml", CodeFormat.Xml);
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\Upgrade\Example.cs", "Example");
			doc.AddCode(@"..\..\..\..\samples\ExtendedXmlSerializer.Samples\bin\Upgrade.Example.v2.xml", CodeFormat.Xml);

			doc.AddHeader("History");
			doc.AddList("2017-??-?? - v2.0.0 - Rewritten version");

			doc.AddHeader("Authors");
			doc.AddList(
				"`Wojciech Nagórski <https://github.com/wojtpl2>`__",
				"`Mike-EEE <https://github.com/Mike-EEE>`__"
			);

			var result = doc.ToString();

			File.WriteAllText("..\\..\\..\\..\\docs\\get-started\\index.rst", result);

			result =
				".. image:: https://img.shields.io/nuget/v/ExtendedXmlSerializer.svg" + Environment.NewLine + "    :target: https://www.nuget.org/packages/ExtendedXmlSerializer/" + Environment.NewLine +
				".. image:: https://ci.appveyor.com/api/projects/status/9u1w8cyyr22kbcwi?svg=true" + Environment.NewLine + "    :target: https://ci.appveyor.com/project/wojtpl2/extendedxmlserializer\n" + Environment.NewLine + Environment.NewLine +
			   result;

			File.WriteAllText("..\\..\\..\\..\\readme.rst", result);
		}
	}
}
