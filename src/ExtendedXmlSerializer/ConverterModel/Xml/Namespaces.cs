using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class Namespaces : CacheBase<string, Namespace>, INamespaces
	{
		readonly IPrefixer _prefixer;

		public Namespaces() : this(new Prefixer()) {}

		public Namespaces(IPrefixer prefixer)
		{
			_prefixer = prefixer;
		}

		protected override Namespace Create(string parameter) => new Namespace(_prefixer.Get(parameter), parameter);
	}
}