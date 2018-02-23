using System.IO;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Instances {
	sealed class RootInstanceCoercer<T> : IParameterizedSource<Stream, T>
	{
		public static RootInstanceCoercer<T> Default { get; } = new RootInstanceCoercer<T>();
		RootInstanceCoercer() : this(RootInstances<T>.Default) {}

		readonly IParameterizedSource<Stream, T> _roots;

		public RootInstanceCoercer(IParameterizedSource<Stream, T> roots) => _roots = roots;

		public T Get(Stream parameter) => _roots.Get(parameter);
	}
}