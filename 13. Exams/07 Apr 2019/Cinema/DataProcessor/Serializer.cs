using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
//using Cinema.DataProcessor.ExportDto;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;
    using Cinema.DataProcessor.ExportDtos;
    using Data;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Rating >= rating * 100 / 100 && m.Projections.Any(p => p.Tickets.Count > 0))
                .ToList()
                .OrderByDescending(m=>m.Rating)
                .ThenBy(m=>m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("f2"),
                    TotalIncomes = m.Projections.Sum(p=>p.Tickets.Sum(t=>t.Price)).ToString("f2"),
                    Customers = m.Projections.SelectMany(p => p.Tickets
                                                            .OrderByDescending(t => t.Customer.Balance)
                                                            .ThenBy(t=>t.Customer.FirstName)
                                                            .ThenBy(t=>t.Customer.LastName)
                                                            .Select(t => new
                                                            {
                                                                FirstName = t.Customer.FirstName,
                                                                LastName = t.Customer.LastName,
                                                                Balance = t.Customer.Balance.ToString("f2")
                                                            })
                                                            .ToList())              
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
                .OrderByDescending(c=>c.Tickets.Sum(t=>t.Price))
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