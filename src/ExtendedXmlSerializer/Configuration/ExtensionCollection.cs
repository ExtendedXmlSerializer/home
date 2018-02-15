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

using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IExtensionCollection : IGroupCollection<ISerializerExtension> { }

	sealed class ExtensionCollection : GroupCollection<ISerializerExtension>, IExtensionCollection
	{
		public ExtensionCollection() : this(DefaultGroups.Default) {}

		public ExtensionCollection(IEnumerable<IGroup<ISerializerExtension>> groups) : base(groups) {}
	}

	sealed class ExtensionCollections : IParameterizedSource<ImmutableArray<ISerializerExtension>, IExtensionCollection>
	{
		public static ExtensionCollections Default { get; } = new ExtensionCollections();
		ExtensionCollections() {}


		public IExtensionCollection Get(ImmutableArray<ISerializerExtension> parameter)
		{
			var result = new ExtensionCollection();
			var command = new AddExtensionCommand(result);
			foreach (var extension in parameter)
			{
				command.Execute(extension);
			}
			return result;
		}
	}

	sealed class RemoveExtensionCommand : ICommand<ISerializerExtension>
	{
		readonly IGroupCollection<ISerializerExtension> _extensions;
		readonly ImmutableArray<GroupName> _names;

		public RemoveExtensionCommand(IGroupCollection<ISerializerExtension> extensions) :
			this(extensions, Categories.Default.ToImmutableArray()) {}

		public RemoveExtensionCommand(IGroupCollection<ISerializerExtension> extensions, ImmutableArray<GroupName> names)
		{
			_extensions = extensions;
			_names = names;
		}

		public void Execute(ISerializerExtension parameter)
		{
			_names.Select(_extensions.Get)
			      .SingleOrDefault(x => x.Contains(parameter))?
			      .Remove(parameter);
		}
	}

	sealed class AddExtensionCommand : AddGroupElementCommand<ISerializerExtension>
	{
		public AddExtensionCommand(IExtensionCollection extensions) : this(Categories.Content, Categories.Default, extensions) {}

		public AddExtensionCommand(GroupName defaultName, ISpecificationSource<string, GroupName> names, IExtensionCollection extensions)
			: base(defaultName, names, extensions) {}
	}
}