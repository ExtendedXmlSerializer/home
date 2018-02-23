using System.IO;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Instances {
	sealed class RootInstances<T> : Cache<Stream, T>
	{
		public static RootInstances<T> Default { get; } = new RootInstances<T>();
		RootInstances() : base(_ => default(T)) {}
	}
}