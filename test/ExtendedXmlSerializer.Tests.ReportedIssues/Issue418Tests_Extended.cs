using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue418Tests_Extended
	{
		[Fact]
		public void Verify()
		{
			var b1 = new Book();
			var b2 = new Book();
			var b3 = new Book();
			var b4 = new Book();
			var b5 = new Book();
			var b6 = new Book();

			var historyBooks = new[] { b1, b2, };
			var artBooks     = new[] { b3, b4, };
			var scienceBooks = new[] { b5 };
			var comics       = new[] { b6 };

			var all = historyBooks.Concat(artBooks).Concat(scienceBooks).Concat(comics).ToList();

			var fs = new BookStore()
			{
				AllBooks = all,
				Genres = new[]
				{
					new Genre()
					{
						Books = historyBooks,
					},
					new Genre()
					{
						Books = historyBooks,
					},
					new Genre()
					{
						Books = artBooks,
					},
					new Genre()
					{
						Books = scienceBooks,
					},
					new Genre()
					{
						Books = comics,
					},
				}
			};

			var serializer = new ConfigurationContainer().Type<Book>(b => b.EnableReferences())
			                                             .Create();
			var cycled = serializer.Cycle(fs);
			cycled.Genres.ElementAt(0).Books.ElementAt(0).Should().BeSameAs(cycled.AllBooks.ElementAt(0));
			cycled.Genres.ElementAt(0).Books.ElementAt(1).Should().BeSameAs(cycled.AllBooks.ElementAt(1));
			cycled.Genres.ElementAt(1).Books.ElementAt(0).Should().BeSameAs(cycled.AllBooks.ElementAt(0));
			cycled.Genres.ElementAt(1).Books.ElementAt(1).Should().BeSameAs(cycled.AllBooks.ElementAt(1));
			cycled.Genres.ElementAt(2).Books.ElementAt(0).Should().BeSameAs(cycled.AllBooks.ElementAt(2));
			cycled.Genres.ElementAt(2).Books.ElementAt(1).Should().BeSameAs(cycled.AllBooks.ElementAt(3));
			cycled.Genres.ElementAt(3).Books.ElementAt(0).Should().BeSameAs(cycled.AllBooks.ElementAt(4));
			cycled.Genres.ElementAt(4).Books.ElementAt(0).Should().BeSameAs(cycled.AllBooks.ElementAt(5));

		}

		class BookStore
		{
			public IEnumerable<Book> AllBooks { get; set; }
			public IEnumerable<Genre> Genres { get; set; }
		}

		class Book
		{
		}

		class Genre
		{
			public IEnumerable<Book> Books { get; set; }
		}

	}
}
