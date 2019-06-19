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
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AllMembersParameterizedActivators : IActivators
	{
		readonly IActivators _activators;
		readonly IQueriedConstructors _constructors;
		readonly IMemberAccessors _accessors;
        readonly ITypeMembers _typeMembers;
        readonly IConstructorMembers _members;

		public AllMembersParameterizedActivators(IActivators activators, IQueriedConstructors constructors,
		                               IConstructorMembers members, IMemberAccessors accessors,
                                       ITypeMembers typeMembers)
		{
			_activators = activators;
			_constructors = constructors;
			_members = members;
			_accessors = accessors;
            _typeMembers = typeMembers;
        }

		public IActivator Get(Type parameter)
		{
			var typeInfo = parameter.GetTypeInfo();
			var constructor = _constructors.Get(typeInfo);
			var members = constructor != null ? _members.Get(constructor) : null;
            var typeMembers = _typeMembers.Get(typeInfo);

			var result = members != null
				? Activator(constructor, typeMembers)
				: _activators.Get(typeInfo);
			return result;
		}

		ActivationContextActivator Activator(ConstructorInfo constructor, ImmutableArray<IMember> members)
		{
			var activator = new Source(constructor).ToDelegate();
			var contexts = new ActivationContexts(_accessors, members, activator);
			var defaults = constructor.GetParameters()
			                          .Where(x => x.IsOptional)
			                          .Select(x => Pairs.Create(x.Name, x.DefaultValue))
			                          .ToImmutableArray();
			var result = new ActivationContextActivator(contexts, defaults);
			return result;
		}

		sealed class Source : IParameterizedSource<Func<string, object>, IActivator>
		{
			readonly ConstructorInfo _constructor;

			public Source(ConstructorInfo constructor) => _constructor = constructor;

			public IActivator Get(Func<string, object> parameter)
			{
				var arguments = new Enumerable<object>(_constructor.GetParameters().Select(x => x.Name).Select(parameter));
				var result = new ConstructedActivator(_constructor, arguments);
				return result;
			}
		}
	}
}