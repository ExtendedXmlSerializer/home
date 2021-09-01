using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Parsing;
using Sprache;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class AssemblyPathParser : Parsing<AssemblyPath>
	{
		readonly static Parser<string>
			Namespace = new Core.Parsing.Identifier('_', '.'),
			Assembly  = new Core.Parsing.Identifier(',', '-', ' ', '_', '.');

		public static AssemblyPathParser Default { get; } = new AssemblyPathParser();

		AssemblyPathParser()
			: base(Parse.String("clr-namespace:")
			            .Then(Namespace.Optional().Accept)
			            .SelectMany(Parse.String(";assembly=")
			                             .Accept, (ns, _) => ns)
			            .SelectMany(Assembly.Accept, (ns, assembly)
				                                         => new AssemblyPath(ns.GetOrDefault(), assembly))) {}
	}
}