// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion.Parsing;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;
using IContents = ExtendedXmlSerializer.ContentModel.IContents;
using XmlReader = System.Xml.XmlReader;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	public sealed class DeferredReferencesExtension : ISerializerExtension
	{
		public static DeferredReferencesExtension Default { get; } = new DeferredReferencesExtension();
		DeferredReferencesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IContentsResult, ContentsResult>()
			            .Decorate<IMemberHandler, Handler>()
			            .Decorate<IReferenceMaps, DeferredReferenceMaps>()
			            .Decorate<ContentModel.Content.IContents, DeferredReferenceContents>()
			            .Decorate<ISerializers, DeferredReferenceSerializers>()
			            .Decorate<IReferenceEncounters, DeferredReferenceEncounters>()
			            .Decorate<IXmlFactory, Factory>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class ContentsResult : IContentsResult
		{
			readonly ICommand<IContents> _command;
			readonly IContentsResult _results;

			[UsedImplicitly]
			public ContentsResult(IContentsResult results) : this(ExecuteDeferredCommandsCommand.Default, results) {}

			public ContentsResult(ICommand<IContents> command, IContentsResult results)
			{
				_command = command;
				_results = results;
			}

			public object Get(IContents parameter)
			{
				_command.Execute(parameter);

				var result = _results.Get(parameter);
				return result;
			}
		}

		[UsedImplicitly]
		sealed class Factory : IXmlFactory
		{
			readonly IXmlFactory _factory;

			public Factory(IXmlFactory factory)
			{
				_factory = factory;
			}

			public IXmlWriter Create(XmlWriter writer, object instance) => _factory.Create(writer, instance);

			public IXmlReader Create(XmlReader reader) => new DeferredReferencesReader(_factory.Create(reader));
		}

		sealed class DeferredReferencesReader : IXmlReader
		{
			readonly IDeferredCommands _commands;
			readonly IXmlReader _reader;

			public DeferredReferencesReader(IXmlReader reader) : this(DeferredCommands.Default, reader) {}

			public DeferredReferencesReader(IDeferredCommands commands, IXmlReader reader)
			{
				_commands = commands;
				_reader = reader;
			}

			public bool IsSatisfiedBy(IIdentity parameter) => _reader.IsSatisfiedBy(parameter);

			public string Identifier => _reader.Identifier;

			public string Name => _reader.Name;

			public IIdentity Get(IIdentity parameter) => _reader.Get(parameter);

			public MemberInfo Get(string parameter) => _reader.Get(parameter);

			public TypeInfo Get(TypeParts parameter) => _reader.Get(parameter);

			public void Dispose()
			{
				var commands = _commands.Get(this);
				if (commands.Any())
				{
					throw new InvalidOperationException(
						"Deferred references were applied to this reader to track, but they were not applied and executed as expected. This is considered an unexpected, invalid, and corrupt state.");
				}
				_reader.Dispose();
			}

			public string Content() => _reader.Content();

			public void Set() => _reader.Set();

			public XmlReader Get() => _reader.Get();
		}

		sealed class Handler : IMemberHandler
		{
			readonly IMemberHandler _handler;

			public Handler(IMemberHandler handler)
			{
				_handler = handler;
			}

			public void Handle(IContents contents, IMemberSerializer member)
			{
				ContentsContext.Default.Assign(contents, member.Access);
				_handler.Handle(contents, member);
				ContentsContext.Default.Remove(contents);
			}
		}
	}
}