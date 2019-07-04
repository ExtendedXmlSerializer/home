using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ExtendedXmlSerializer.Samples.Generics
{
    public class GenericSerializer
    {
        private IExtendedXmlSerializer serializer;

        public GenericSerializer()
        {
            serializer = new ConfigurationContainer()
                                            .EnableAllConstructors()
                                            .EnableParameterizedContentWithPropertyAssignments()
                                            .UseOptimizedNamespaces()
                                            .EnableReferences()
                                            .UseOptimizedNamespaces()
                                            .Create();
        }

        public T DeserializeProject<T>(string serializedProject)
        {
            return serializer.Deserialize<T>(new XmlReaderSettings { IgnoreWhitespace = true }, serializedProject);
        }

        public string SerializeProject<T>(T project)
        {
            return serializer.Serialize(new XmlWriterSettings { Indent = true }, project);
        }

        public static void SerializeAndDeserialize()
        {
            var srl = new GenericSerializer();
            var prj = new Project("test project", 2);

            var cls = new SpecificClass();
            cls.FirstRecipe = new FirstRecipe();
            cls.Context.Value = "asfr";
            cls.UserDescription = "user descriptio";
            cls.FirstRecipe.ActivityParametersList.Add(new MyActivityParameters(17));
            cls.SecondRecipe = new SecondRecipe();
            cls.SecondRecipe.ActivityParametersList.Add(new MySecondActivityParameters());
            prj.Sites.Add(cls);
            prj.Name = "custom name";


            var data = srl.SerializeProject(prj);
            var obj = srl.DeserializeProject<IProject>(data);
        }


        public interface IProject
        {
            string Name { get; }
            string Path { get; }
            IList<ISite<IContext>> Sites { get; }
        }

        public interface ISite<out TSiteContext> where TSiteContext : class, IContext
        {
            string Name { get; set; }
            string UserDescription { get; set; }

            TSiteContext Context { get; }
        }

        public interface IContext
        {
            string Value { get; }
        }

        public class Project : IProject
        {
            public Project(string name, int id)
            {
                Name = name;
                Id = id;
            }

            public string Name { get; set; } = "MY project";
            public int Id { get; }
            public string Path => @"C:/my/fake/path";

            public IList<ISite<IContext>> Sites { get; } = new List<ISite<IContext>>();
        }

        public class SpecificClass : SiteBase<Context>
        {
            public SpecificClass()
            {
                this.Name = "Specific class";
                this.UserDescription = "Class description";
                this.Context = new Context();
            }


            public override Context Context { get; set; }

            public FirstRecipe FirstRecipe { get; set; } = new FirstRecipe();
            public SecondRecipe SecondRecipe { get; set; } = new SecondRecipe();
        }

        public abstract class SiteBase<TSiteContext> : ISite<TSiteContext> where TSiteContext : class, IContext
        {
            public string Name { get; set; }
            public string UserDescription { get; set; }
            public abstract TSiteContext Context { get; set; }
        }

        public class Context : IContext
        {
            public string Value { get; set; } = "My context";
        }

        public interface IRecipe
        {
            string Name { get; }
            IEnumerable<IActivityParameters> ActivityParameters { get; }
        }

        public abstract class RecipeBase : IRecipe
        {
            public abstract string Name { get; }
            public IEnumerable<IActivityParameters> ActivityParameters => ActivityParametersList;
            public List<IActivityParameters> ActivityParametersList { get; } = new List<IActivityParameters>();
        }

        public class FirstRecipe : RecipeBase
        {
            public override string Name => nameof(FirstRecipe);

            public double Lenght { get; set; } = 23;

            public FirstRecipe()
            {
            }
        }

        public class SecondRecipe : RecipeBase
        {
            public override string Name => nameof(SecondRecipe);

            public double Thickness { get; set; } = 100;

            public SecondRecipe()
            {
            }
        }

        public interface IActivityParameters
        {
            bool IsEnabled { get; set; }
        }

        public class MyActivityParameters : IActivityParameters
        {
            public MyActivityParameters(int propA)
            {
                PropA = propA;
                MyProperty = propA;
            }

            public int PropA { get; }

            public int MyProperty { get; set; }
            public bool IsEnabled { get; set; }
        }

        public class MySecondActivityParameters : IActivityParameters
        {
            public int MyProperty2 { get; set; }
            public bool IsEnabled { get; set; }
        }
    }
}
