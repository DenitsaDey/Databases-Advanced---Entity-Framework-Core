namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int command = int.Parse(Console.ReadLine());
            int result = RemoveBooks(db);

            Console.WriteLine(result);
        }

        //2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            List<string> bookTitles = context
                .Books
                .ToList()  //AsEnumerable()
                .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(b=>b.Title)
                .OrderBy(bt=>bt)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            List<string> goldenEdition = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, goldenEdition);
        }

        //4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var bookTitles = context
                .Books                
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var item in bookTitles)
            {
                sb.AppendLine($"{item.Title} - ${item.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            List<string> bookTitles = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            string[] categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c=>c.ToLower())
                .ToArray();

            List<string> bookTitles = new List<string>();

            foreach (string category in categories)
            {
                List<string> booksInCategory = context.Books
                    .Where(b => b.BookCategories.Any(bc =>bc.Category.Name.ToLower() == category))
                    .Select(b => b.Title)
                    .ToList();

                bookTitles.AddRange(booksInCategory);
            }

            bookTitles = bookTitles.OrderBy(bt => bt).ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //7. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();
            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var booksReleasedBeforeDate = context
                .Books
                .Where(b => b.ReleaseDate.Value < releaseDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            foreach (var book in booksReleasedBeforeDate)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //8. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine(author.FullName);
            }

            return sb.ToString().TrimEnd();
        }

        //9. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            List<string> bookTitles = context
               .Books
               .Where(b => b.Title.ToLower().Contains(input.ToLower())) //(b=>b.Title.Contains(input, StringComparison.InvariantCultureIgnoreCase)
               .Select(b => b.Title)
               .OrderBy(bt => bt)
               .ToList();

            return string.Join(Environment.NewLine, bookTitles);
        }

        //10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var booksByAuthor = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    b.Title,
                    Author = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            foreach (var book in booksByAuthor)
            {
                sb.AppendLine($"{book.Title} ({book.Author})");
            }

            return sb.ToString().TrimEnd();
        }

        //11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int books = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return books;
        }

        //12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            var authorCopies = context
                .Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    Count = a.Books.Sum(b=>b.Copies)
                })
                .OrderByDescending(a=>a.Count)
                .ToList();

            foreach (var author in authorCopies)
            {
                sb.AppendLine($"{author.FullName} - {author.Count}");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var booksByCategory = context
                .Categories
                .Select(c => new
                {
                    Category = c.Name,
                    Profit = c.CategoryBooks
                    .Select(cb=> new
                    {
                        BookProfit = cb.Book.Price * cb.Book.Copies
                    })
                    .Sum(c=>c.BookProfit)
                })
                .OrderByDescending(c=>c.Profit)
                .ThenBy(c=>c.Category)
                .ToList();

            foreach (var item in booksByCategory)
            {
                sb.AppendLine($"{item.Category} ${item.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var booksByCategory = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    MostRecent = c.CategoryBooks
                       .OrderByDescending(cb=>cb.Book.ReleaseDate)
                       .Take(3)
                    .Select(cb => new
                    {
                        Title = cb.Book.Title,
                        Released = cb.Book.ReleaseDate.Value.Year
                    })
                    .ToList()
                })
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var c in booksByCategory)
            {
                sb.AppendLine($"--{c.Name}");
                foreach (var b in c.MostRecent)
                {
                    sb.AppendLine($"{b.Title} ({b.Released})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var booksIncreased = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in booksIncreased)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksDeleted = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(booksDeleted);
            context.SaveChanges();

            return booksDeleted.Count();
        }
    }
}
