namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var books = XmlConverter.Deserializer<BookInputModel>(xmlString, "Books");

            foreach (var currentBook in books)
            {
                var isValidPublishDate = DateTime.TryParseExact(
                    currentBook.PublishedOn,
                    "MM/dd/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime currentPublishDate);

                bool isValidGenre = Enum.TryParse(currentBook.Genre, out Genre currentGenre);

                if(!IsValid(currentBook) ||
                    !isValidPublishDate ||
                    !isValidGenre)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var bookToImport = new Book
                {
                    Name = currentBook.Name,
                    Genre = currentGenre,
                    Pages = currentBook.Pages,
                    Price = currentBook.Price,
                    PublishedOn = currentPublishDate
                };

                context.Books.Add(bookToImport);
                sb.AppendLine(string.Format(SuccessfullyImportedBook, bookToImport.Name, bookToImport.Price));
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var authors = JsonConvert.DeserializeObject<IEnumerable<AuthorInputModel>>(jsonString);

            foreach (var currentAuthor in authors)
            {
                if (!IsValid(currentAuthor) ||
                    context.Authors.Any(a=>a.Email == currentAuthor.Email))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var authorToImport = new Author
                {
                    FirstName = currentAuthor.FirstName,
                    LastName = currentAuthor.LastName,
                    Email = currentAuthor.Email,
                    Phone = currentAuthor.Phone
                };

                foreach (var book in currentAuthor.Books)
                {
                    if (!book.Id.HasValue)
                    {
                        continue;
                    }

                    Book currentBook = context.Books.FirstOrDefault(b => b.Id == book.Id);
                    if(currentBook == null)
                    {
                        continue;
                    }

                    authorToImport.AuthorsBooks.Add(new AuthorBook 
                    { 
                        Book = currentBook,
                        Author = authorToImport
                    });
                }

                if (!authorToImport.AuthorsBooks.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                context.Authors.Add(authorToImport);
                sb.AppendLine(string.Format(SuccessfullyImportedAuthor, $"{authorToImport.FirstName} {authorToImport.LastName}", authorToImport.AuthorsBooks.Count));
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}