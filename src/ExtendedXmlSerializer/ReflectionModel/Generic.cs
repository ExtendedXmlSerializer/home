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

using System;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	class Generic<T> : GenericAdapterBase<Func<T>>, IGeneric<T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T>> Activator =
			new GenericActivators<Func<T>>(GenericSingleton.Default).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) { }

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T>> source) : base(definition, source) { }
	}

	class Generic<T1, T> : GenericAdapterBase<Func<T1, T>>, IGeneric<T1, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T>> Activator
			= new GenericActivators<Func<T1, T>>(typeof(T1)).ReferenceCache();


		public Generic(Type definition) : this(definition, Activator) { }

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T>> source) : base(definition, source) { }
	}

	class Generic<T1, T2, T> : GenericAdapterBase<Func<T1, T2, T>>, IGeneric<T1, T2, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T2, T>> Activator
			= new GenericActivators<Func<T1, T2, T>>(typeof(T1), typeof(T2)).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) { }

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T2, T>> source)
			: base(definition, source) { }
	}

	class Generic<T1, T2, T3, T> : GenericAdapterBase<Func<T1, T2, T3, T>>, IGeneric<T1, T2, T3, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T2, T3, T>> Activator
			= new GenericActivators<Func<T1, T2, T3, T>>(typeof(T1), typeof(T2), typeof(T3)).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) { }

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T2, T3, T>> source)
			: base(definition, source) { }
	}

	class Generic<T1, T2, T3, T4, T> : GenericAdapterBase<Func<T1, T2, T3, T4, T>>, IGeneric<T1, T2, T3, T4, T>
	{
		readonly static IParameterizedSource<TypeInfo, Func<T1, T2, T3, T4, T>> Activator
			= new GenericActivators<Func<T1, T2, T3, T4, T>>(typeof(T1), typeof(T2), typeof(T3), typeof(T4)).ReferenceCache();

		public Generic(Type definition) : this(definition, Activator) { }

		public Generic(Type definition, IParameterizedSource<TypeInfo, Func<T1, T2, T3, T4, T>> source)
			: base(definition, source) { }
	}
}