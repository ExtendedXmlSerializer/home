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


namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class TableEntries<TParameter, TResult> : ReferenceCacheBase<TParameter, IEntry<TResult>>
		where TParameter : class
	{
		public static IParameterizedSource<ITableSource<TParameter, TResult>, TableEntries<TParameter, TResult>>
			Defaults {get;} = new ReferenceCache<ITableSource<TParameter, TResult>, TableEntries<TParameter, TResult>>(x => new TableEntries<TParameter, TResult>(x));

		readonly ITableSource<TParameter, TResult> _table;

		public TableEntries(ITableSource<TParameter, TResult> table) => _table = table;

		protected override IEntry<TResult> Create(TParameter parameter)
			=> new TableEntry<TParameter, TResult>(_table, parameter);
	}


	public interface IEntry<T> : ISource<T>, ICommand<T>
	{
		ICommand Remove { get; }
	}

	sealed class TableEntry<TParameter, TResult> : FixedParameterSource<TParameter, TResult>, IEntry<TResult>
	{
		readonly ICommand<TResult> _assign;

		public TableEntry(ITableSource<TParameter, TResult> table, TParameter parameter)
			: this(table, parameter, new FixedAssignment<TParameter, TResult>(table, parameter),
			       new Remove<TParameter, TResult>(table, parameter)) {}

		public TableEntry(IParameterizedSource<TParameter, TResult> table, TParameter parameter, ICommand<TResult> assign, ICommand remove) : base(table, parameter)
		{
			_assign = assign;
			Remove = remove;
		}

		public void Execute(TResult parameter)
		{
			_assign.Execute(parameter);
		}

		public ICommand Remove { get; }
	}
}