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
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core
{
	public class CompositeCommand<T> : ICommand<T>
	{
		readonly ImmutableArray<ICommand<T>> _items;
		readonly int _length;
		public CompositeCommand(params ICommand<T>[] items) : this(items.ToImmutableArray()) {}

		public CompositeCommand(ImmutableArray<ICommand<T>> items) : this(items, items.Length) {}

		public CompositeCommand(ImmutableArray<ICommand<T>> items, int length)
		{
			_items = items;
			_length = length;
		}

		public void Execute(T parameter)
		{
			for (var i = 0; i < _length; i++)
			{
				_items[i].Execute(parameter);
			}
		}
	}

	public class DeferredInstanceCommand<T> : ICommand<T>
	{
		readonly Lazy<ICommand<T>> _command;
		public DeferredInstanceCommand(Func<ICommand<T>> command) : this(new Lazy<ICommand<T>>(command)) {}
		public DeferredInstanceCommand(Lazy<ICommand<T>> command) => _command = command;

		public void Execute(T parameter)
		{
			_command.Value.Execute(parameter);
		}
	}

	class DecoratedCommand<T> : DelegatedCommand<T>
	{
		public DecoratedCommand(ICommand<T> action) : base(action.Execute) {}
	}

	class FixedCommand<T> : ICommand<T>
	{
		readonly ICommand<T> _command;
		readonly T _parameter;

		public FixedCommand(ICommand<T> command, T parameter)
		{
			_command = command;
			_parameter = parameter;
		}

		public void Execute(T _)
		{
			_command.Execute(_parameter);
		}
	}

	class DelegatedCommand<T> : ICommand<T>
	{
		readonly Action<T> _action;

		public DelegatedCommand(Action<T> action) => _action = action;

		public void Execute(T parameter)
		{
			_action(parameter);
		}
	}

	class DelegatedSourceCommand<TParameter, T> : ICommand<TParameter>
	{
		readonly Func<TParameter, ICommand<T>> _source;
		readonly Func<TParameter, T> _coercer;

		public DelegatedSourceCommand(Func<TParameter, ICommand<T>> source, Func<TParameter, T> coercer)
		{
			_source = source;
			_coercer = coercer;
		}

		public void Execute(TParameter parameter)
		{
			_source(parameter).Execute(_coercer(parameter));
		}
	}

	public class DeferredCommand<T> : ICommand<T>
	{
		readonly Func<ICommand<T>> _command;
		public DeferredCommand(Func<ICommand<T>> command) => _command = command;

		public void Execute(T parameter)
		{
			_command().Execute(parameter);
		}
	}
}