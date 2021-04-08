namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                            .OrderByDescending(ab => ab.Book.Price)
                            .Select(ab => new
                            {
                                BookName = ab.Book.Name,
                                BookPrice = ab.Book.Price.ToString("f2")
                            })
                            .ToList()
                })
                .ToList()
                .OrderByDescending(a => a.Books.Count)
                .ThenBy(a => a.AuthorName);

            string json = JsonConvert.SerializeObject(authors, Formatting.Indented);
            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .Where(b => b.Genre.ToString() == "Science" && b.PublishedOn < date)
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn)
                .Take(10)
                .Select(b => new BookExportModel
                {
                    Pages = b.Pages,
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture)
                })
                .ToList();

            var xml = XmlConverter.Serialize(books, "Books");
            return xml;
        }
    }
}