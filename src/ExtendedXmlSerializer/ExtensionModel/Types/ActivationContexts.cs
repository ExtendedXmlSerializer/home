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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContexts : IActivationContexts
	{
		readonly IMemberAccessors                       _accessors;
		readonly ImmutableArray<IMember>                _members;
		readonly Func<Func<string, object>, IActivator> _activator;

		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members, IActivator activator)
			: this(accessors, members, activator.Accept) {}

		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members,
		                          Func<Func<string, object>, IActivator> activator)
		{
			_accessors = accessors;
			_members   = members;
			_activator = activator;
		}

		public IActivationContext Get(IDictionary<string, object> parameter)
		{
			var source = new TableSource<string, object>(parameter);
			var list   = new List<object>();
			var command = new CompositeCommand<object>(new ApplyMemberValuesCommand(_accessors, _members, source),
			                                           new AddItemsCommand(list));
			var alteration = new ConfiguringAlteration<object>(command);
			var members    = _members.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
			var store      = new Store(source, new TableSource<string, IMember>(members));
			var activator  = new AlteringActivator(alteration, _activator.Invoke(store.Get));
			var result = new ActivationContext(source, activator.Singleton()
			                                                    .Get, list);
			return result;
		}

		sealed class Store : IParameterizedSource<string, object>
		{
			readonly ITableSource<string, object>          _store;
			readonly IParameterizedSource<string, IMember> _members;
			readonly ITypeDefaults                         _defaults;

			public Store(ITableSource<string, object> store, IParameterizedSource<string, IMember> members)
				: this(store, members, TypeDefaults.Default) {}

			public Store(ITableSource<string, object> store, IParameterizedSource<string, IMember> members,
			             ITypeDefaults defaults)
			{
				_store    = store;
				_members  = members;
				_defaults = defaults;
			}

			public object Get(string parameter)
				=> _store.IsSatisfiedBy(parameter) ? _store.Get(parameter) : Default(parameter);

			object Default(string parameter)
			{
				var member = _members.Get(parameter);
				var result = _defaults.Get(member.MemberType);
				return result;
			}
		}
	}
}