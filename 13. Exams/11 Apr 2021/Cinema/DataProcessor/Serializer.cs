namespace Cinema.DataProcessor
{
    using System;
    using System.Linq;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                 .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Count > 0))
                 .ToList()
                 .OrderByDescending(m => m.Rating)
                 .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                 .Select(m => new
                 {
                     MovieName = m.Title,
                     Rating = m.Rating.ToString("f2"),
                     TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("f2"),
                     Customers = m.Projections.SelectMany(p => p.Tickets
                                                                .Select(t => new
                                                             {
                                                                 FirstName = t.Customer.FirstName,
                                                                 LastName = t.Customer.LastName,
                                                                 Balance = t.Customer.Balance.ToString("f2")
                                                             })
                                                                .ToList())
                                                             .OrderByDescending(t => t.Balance)
                                                             .ThenBy(t => t.FirstName)
                                                             .ThenBy(t => t.LastName)
                                                             .ToList()
                 })
                 .Take(10)
                 .ToList();

            string json = JsonConvert.SerializeObject(movies, Formatting.Indented);
            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .ToList()
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Select(c => new CustomerXmlOutputModel
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("f2"),
                    SpentTime = TimeSpan.FromMilliseconds(c.Tickets.Sum(t => t.Projection.Movie.Duration.TotalMilliseconds)).ToString("c")
                })
                .Take(10)
                .ToList();

            string xml = XmlConverter.Serialize(customers, "Customers");

            return xml;
        }
    }
}