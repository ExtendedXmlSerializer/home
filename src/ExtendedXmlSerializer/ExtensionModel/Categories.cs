using ExtendedXmlSerializer.Core.Collections;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class Categories : GroupNames
	{
		public static GroupName Start           = new GroupName("Start"),
		                        ReflectionModel = new GroupName("Reflection Model"),
		                        ContentModel    = new GroupName("Content Model"),
		                        ObjectModel     = new GroupName("Object Model"),
		                        Framework       = new GroupName("Framework"),
		                        Elements        = new GroupName("Elements"),
		                        Content         = new GroupName("Content"),
		                        Registrations   = new GroupName("Registrations"),
		                        Format          = new GroupName("Format"),
		                        Alterations     = new GroupName("Alterations"),
		                        Caching         = new GroupName("Caching"),
		                        Finish          = new GroupName("Finish");

		public static Categories Default { get; } = new Categories();

		Categories() : this(Start, ReflectionModel, ContentModel, ObjectModel, Framework, Elements, Content, Registrations,
		                    Format, Alterations, Caching, Finish) {}

		public Categories(params GroupName[] names) : base(names) {}
	}
}