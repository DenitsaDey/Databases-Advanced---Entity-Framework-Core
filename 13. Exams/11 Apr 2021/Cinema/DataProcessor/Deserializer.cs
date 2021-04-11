namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";

        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";

        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            List<Movie> moviesToImport = new List<Movie>();

            var movies = JsonConvert.DeserializeObject<IEnumerable<MovieJsonInputModel>>(jsonString);

            foreach (var currentMovie in movies)
            {
                bool isGenreValid = Enum.TryParse<Genre>(currentMovie.Genre, out Genre currentGenre);
                bool isDurationValid = TimeSpan.TryParseExact(
                    currentMovie.Duration,
                    "c",
                    CultureInfo.InvariantCulture,
                    TimeSpanStyles.None,
                    out TimeSpan currentDuration);

                if (!IsValid(currentMovie) ||
                    !isGenreValid ||
                    !isDurationValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (moviesToImport.Any(m => m.Title == currentMovie.Title))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movieToAdd = new Movie
                {
                    Title = currentMovie.Title,
                    Genre = currentGenre,
                    Duration = currentDuration,
                    Rating = currentMovie.Rating,
                    Director = currentMovie.Director
                };

                moviesToImport.Add(movieToAdd);
                sb.AppendLine(string.Format(SuccessfulImportMovie, movieToAdd.Title, movieToAdd.Genre, movieToAdd.Rating.ToString("f2")));
            }
            context.Movies.AddRange(moviesToImport);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var projections = XmlConverter.Deserializer<ProjectionXmlInputModel>(xmlString, "Projections");
            List<Projection> projectionsToImport = new List<Projection>();

            foreach (var currentProjection in projections)
            {
                bool isValidDateTime = DateTime.TryParseExact(
                    currentProjection.DateTime,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime currentDateTime);

                var currentMovie = context.Movies.FirstOrDefault(m => m.Id == currentProjection.MovieId);

                if (!IsValid(currentProjection) ||
                    !isValidDateTime ||
                    currentMovie == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projectionToAdd = new Projection
                {
                    MovieId = currentProjection.MovieId,
                    DateTime = currentDateTime
                };

                projectionsToImport.Add(projectionToAdd);
                sb.AppendLine(string.Format(SuccessfulImportProjection, currentMovie.Title, projectionToAdd.DateTime.ToString("MM/dd/yyyy")));
            }
            context.Projections.AddRange(projectionsToImport);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var customers = XmlConverter.Deserializer<CustomerXmlInputModel>(xmlString, "Customers");
            List<Customer> customersToImport = new List<Customer>();

            foreach (var currentCustomer in customers)
            {
                if (!IsValid(currentCustomer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customerToAdd = new Customer
                {
                    FirstName = currentCustomer.FirstName,
                    LastName = currentCustomer.LastName,
                    Age = currentCustomer.Age,
                    Balance = currentCustomer.Balance
                };

                foreach (var currentTicket in currentCustomer.Tickets)
                {
                    var currentProjection = context.Projections.FirstOrDefault(p => p.Id == currentTicket.ProjectionId);
                    if (!IsValid(currentTicket) ||
                        currentProjection == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var ticketToAdd = new Ticket
                    {
                        Projection = currentProjection,
                        Price = currentTicket.Price
                    };
                    customerToAdd.Tickets.Add(ticketToAdd);
                }
                customersToImport.Add(customerToAdd);
                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customerToAdd.FirstName, customerToAdd.LastName, customerToAdd.Tickets.Count));
            }
            context.Customers.AddRange(customersToImport);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}