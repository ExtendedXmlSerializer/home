using System;
using System.IO;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Instances {
	sealed class RootInstanceTypes : Cache<Stream, Type>
	{
		public static RootInstanceTypes Default { get; } = new RootInstanceTypes();
		RootInstanceTypes() : base(_ => null) {}
	}
}