using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class SingletonCandidates : Items<string>, ISingletonCandidates
	{
		public static SingletonCandidates Default { get; } = new SingletonCandidates();

		SingletonCandidates() : this("Default", "Instance", "Implementation", "Singleton") {}

		public SingletonCandidates(params string[] items) : base(items) {}
	}
}