using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Sources;
using Sprache;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Markup
{
	sealed class MarkupExtensionAwareSerializer : ContentModel.ISerializer
	{
		readonly Parser<MarkupExtensionParts> _parser;
		readonly IMarkupExtensions            _container;
		readonly ContentModel.ISerializer                  _serializer;

		public MarkupExtensionAwareSerializer(IMarkupExtensions container, ContentModel.ISerializer serializer)
			: this(MarkupExtensionParser.Default, container, serializer) {}

		public MarkupExtensionAwareSerializer(Parser<MarkupExtensionParts> parser, IMarkupExtensions container,
		                                      ContentModel.ISerializer serializer)
		{
			_parser     = parser;
			_container  = container;
			_serializer = serializer;
		}

		public object Get(IFormatReader parameter)
		{
			var candidate = Candidate(parameter);
			var parts     = candidate.Instance as MarkupExtensionParts;
			var result = parts != null
				             ? _container.Get(parameter)
				                         .Get(parts)
				             : candidate.Get();
			return result;
		}

		CandidateResult Candidate(IFormatReader parameter)
		{
			try
			{
				return new CandidateResult(_serializer.Get(parameter));
			}
			catch (Exception e)
			{
				return new CandidateResult(_parser.Parse(parameter.Content()), e);
			}
		}

		readonly struct CandidateResult : ISource<object>
		{
			readonly Exception _error;

			public CandidateResult(object instance, Exception error = null)
			{
				_error   = error;
				Instance = instance;
			}

			public object Instance { get; }

			public object Get()
			{
				if (_error != null)
				{
					// ReSharper disable once ThrowingSystemException
					// ReSharper disable once UnthrowableException
					throw _error;
				}

				return Instance;
			}
		}

		public void Write(IFormatWriter writer, object instance) => _serializer.Write(writer, instance);
	}
}