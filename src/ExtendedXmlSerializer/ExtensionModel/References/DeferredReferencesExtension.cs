// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Services;
using JetBrains.Annotations;
using System;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferencesExtension : ISerializerExtension, ISortAware
	{
		public static DeferredReferencesExtension Default { get; } = new DeferredReferencesExtension();
		DeferredReferencesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IInnerContentResult, InnerContentResult>()
			            .Decorate<IMemberHandler, Handler>()
			            .Decorate<IReferenceMaps, DeferredReferenceMaps>()
			            .Decorate<IContents, DeferredReferenceContents>()
			            .Decorate<ContentModel.Content.ISerializers, DeferredReferenceSerializers>()
			            .Decorate<IReferenceEncounters, DeferredReferenceEncounters>()
			            .Decorate(typeof(IFormatReaders<>), typeof(Factory<>));

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class InnerContentResult : IInnerContentResult
		{
			readonly ICommand<IInnerContent> _command;
			readonly IInnerContentResult _results;

			[UsedImplicitly]
			public InnerContentResult(IInnerContentResult results) : this(ExecuteDeferredCommandsCommand.Default, results) {}

			public InnerContentResult(ICommand<IInnerContent> command, IInnerContentResult results)
			{
				_command = command;
				_results = results;
			}

			public object Get(IInnerContent parameter)
			{
				_command.Execute(parameter);
				return _results.Get(parameter);
			}
		}

		[UsedImplicitly]
		sealed class Factory<T> : IFormatReaders<T>
		{
			readonly IFormatReaders<T> _factory;

			public Factory(IFormatReaders<T> factory)
			{
				_factory = factory;
			}

			public IFormatReader Get(T reader) => new DeferredReferencesReader(_factory.Get(reader));
		}

		sealed class DeferredReferencesReader : IFormatReader
		{
			readonly IDeferredCommands _commands;
			readonly IFormatReader _reader;

			public DeferredReferencesReader(IFormatReader reader) : this(DeferredCommands.Default, reader) {}

			public DeferredReferencesReader(IDeferredCommands commands, IFormatReader reader)
			{
				_commands = commands;
				_reader = reader;
			}

			public bool IsSatisfiedBy(IIdentity parameter) => _reader.IsSatisfiedBy(parameter);

			public string Identifier => _reader.Identifier;

			public string Name => _reader.Name;

			public IIdentity Get(string name, string identifier) => _reader.Get(name, identifier);

			public MemberInfo Get(string parameter) => _reader.Get(parameter);

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
			public object Get() => _reader.Get();
		}

		sealed class Handler : IMemberHandler
		{
			readonly IMemberHandler _handler;

			public Handler(IMemberHandler handler)
			{
				_handler = handler;
			}

			public void Handle(IInnerContent contents, IMemberSerializer member)
			{
				ContentsContext.Default.Assign(contents, member.Access);
				_handler.Handle(contents, member);
				ContentsContext.Default.Remove(contents);
			}
		}

		public int Sort => 2;
	}
}