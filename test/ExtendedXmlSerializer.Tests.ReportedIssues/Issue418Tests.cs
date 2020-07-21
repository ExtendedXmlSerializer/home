using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue418Tests
	{
		[Fact]
		public void Verify()
		{
			var b1 = new Book();
			var b2 = new Book();

			var allBooks = new[] {b1, b2,};

			var fs = new BookStore()
			{
				AllBooks = allBooks,
				Genres = new[]
				{
					new Genre
					{
						Books = allBooks,
					},
					new Genre
					{
						Books = allBooks,
					},
				}
			};

			var serializer = new ConfigurationContainer().Type<Book[]>()
			                                             .EnableReferences()
			                                             .Create()
			                                             .ForTesting();

			var cycled = serializer.Cycle(fs);

			cycled.AllBooks.Should().BeSameAs(cycled.Genres.ElementAt(0).Books);
			cycled.AllBooks.Should().BeSameAs(cycled.Genres.ElementAt(1).Books);
		}

		class BookStore
		{
			public IEnumerable<Book> AllBooks { get; set; }
			public IEnumerable<Genre> Genres { get; set; }
		}

		class Book {}

		class Genre
		{
			public IEnumerable<Book> Books { get; set; }
		}
	}
}