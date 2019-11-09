using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class OptimizedNamespaceXmlWriter : IFormatWriter
	{
		readonly IFormatWriter            _writer;
		readonly ICommand<IIdentityStore> _command;

		public OptimizedNamespaceXmlWriter(IFormatWriter writer, ICommand<IIdentityStore> command)
		{
			_writer  = writer;
			_command = command;
		}

		public object Get() => _writer.Get();

		public void Start(IIdentity identity)
		{
			_writer.Start(identity);

			_command.Execute(_writer);
		}

		public void EndCurrent() => _writer.EndCurrent();

		public void Content(IIdentity property, string content) => _writer.Content(property, content);

		public void Content(string content) => _writer.Content(content);

		public void Verbatim(string content)
		{
			_writer.Verbatim(content);
		}

		public string Get(MemberInfo parameter) => _writer.Get(parameter);

		public void Dispose() => _writer.Dispose();

		public IIdentity Get(string name, string identifier) => _writer.Get(name, identifier);
	}
}