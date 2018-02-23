using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Alterations {
	sealed class ContentPipelineFormatWriter : IFormatWriter
	{
		readonly IFormatWriter     _writer;
		readonly IPipeline<string> _pipeline;

		public ContentPipelineFormatWriter(IFormatWriter writer, IPipeline<string> pipeline)
		{
			_writer   = writer;
			_pipeline = pipeline;
		}

		public object Get() => _writer.Get();

		public IIdentity Get(string name, string identifier) => _writer.Get(name, identifier);

		public void Dispose()
		{
			_writer.Dispose();
		}

		public string Get(MemberInfo parameter) => _writer.Get(parameter);

		public void Start(IIdentity identity)
		{
			_writer.Start(identity);
		}

		public void EndCurrent()
		{
			_writer.EndCurrent();
		}

		public void Content(IIdentity property, string content)
		{
			_writer.Content(property, content);
		}

		public void Content(string content)
		{
			_writer.Content(content == null ? null : _pipeline.Alter(content));
		}

		public void Verbatim(string content)
		{
			_writer.Verbatim(content);
		}
	}
}