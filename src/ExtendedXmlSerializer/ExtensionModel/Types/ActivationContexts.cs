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
using System.Collections.Generic;
using System.Collections.Immutable;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContexts : IActivationContexts
	{
		readonly IMemberAccessors _accessors;
		readonly ImmutableArray<IMember> _members;
		readonly Func<Func<string, object>, IActivator> _activator;

		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members, IActivator activator)
			: this(accessors, members, activator.Accept) {}

		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members,
		                          Func<Func<string, object>, IActivator> activator)
		{
			_accessors = accessors;
			_members = members;
			_activator = activator;
		}

		public IActivationContext Get(IDictionary<string, object> parameter)
		{
			var source = new TableSource<string, object>(parameter);
			var command = new ApplyMemberValuesCommand(_accessors, _members, source);
			var alteration = new ConfiguringAlteration<object>(command);
			var activator = new AlteringActivator(alteration, _activator.Invoke(source.Get));
			var result = new ActivationContext(source, activator);
			return result;
		}
	}
}