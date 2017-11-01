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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedActivators : IActivators
	{
		readonly IActivators _activators;
		readonly IQueriedConstructors _constructors;
		readonly IMemberAccessors _accessors;
		readonly IConstructorMembers _members;

		public ParameterizedActivators(IActivators activators, IQueriedConstructors constructors,
		                               IConstructorMembers members, IMemberAccessors accessors)
		{
			_activators = activators;
			_constructors = constructors;
			_members = members;
			_accessors = accessors;
		}

		public IActivator Get(Type parameter)
		{
			var typeInfo = parameter.GetTypeInfo();
			var constructor = _constructors.Get(typeInfo);
			var members = constructor != null ? _members.Get(constructor) : null;
			var result = members != null
				? Activator(constructor, members.GetValueOrDefault())
				: _activators.Get(typeInfo);
			return result;
		}

		ActivationContextActivator Activator(ConstructorInfo constructor, ImmutableArray<IMember> members)
			=> new ActivationContextActivator(new ActivationContexts(_accessors, members, new Source(constructor).Get));

		sealed class Source : IParameterizedSource<Func<string, object>, IActivator>
		{
			readonly ConstructorInfo _constructor;

			public Source(ConstructorInfo constructor)
			{
				_constructor = constructor;
			}

			public IActivator Get(Func<string, object> parameter)
			{
				var arguments = new Enumerable<object>(_constructor.GetParameters().Select(x => x.Name).Select(parameter));
				var result = new ConstructedActivator(_constructor, arguments);
				return result;
			}
		}
	}
}