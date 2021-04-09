using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
//using Cinema.Data.Models;
//using Cinema.Data.Models.Enums;
//using Cinema.DataProcessor.ImportDto;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Cinema.DataProcessor
{
    using System;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDtos;
    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

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
                if(!IsValid(currentMovie) ||
                    !isGenreValid ||
                    !isDurationValid)
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

                if(context.Movies.Any(m=>m.Title == movieToAdd.Title))
                {
                    sb.AppendLine(ErrorMessage);
                    break;
                }

                context.Movies.Add(movieToAdd);
                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfulImportMovie, movieToAdd.Title, movieToAdd.Genre, movieToAdd.Rating.ToString("f2")));
            }

            return sb.ToString().Trim();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var halls = JsonConvert.DeserializeObject<IEnumerable<HallJsonInputModel>>(jsonString);
            foreach (var currnetHall in halls)
            {
                if (!IsValid(currnetHall))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hallToAdd = new Hall
                {
                    Name = currnetHall.Name,
                    Is4Dx = currnetHall.Is4Dx,
                    Is3D = currnetHall.Is3D,
                };

                for (int i = 0; i < currnetHall.Seats; i++)
                {
                    hallToAdd.Seats.Add(new Seat { Hall = hallToAdd });
                }

                string projectionType;
                if (hallToAdd.Is4Dx && hallToAdd.Is3D)
                {
                    projectionType = "4Dx/3D";
                }
                else if (hallToAdd.Is3D && !hallToAdd.Is4Dx)
                {
                    projectionType = "3D";
                }
                else if (hallToAdd.Is4Dx && !hallToAdd.Is3D)
                {
                    projectionType = "4Dx";
                }
                else
                {
                    projectionType = "Normal";
                }

                context.Halls.Add(hallToAdd);
                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hallToAdd.Name, projectionType, hallToAdd.Seats.Count));
            }

            return sb.ToString().Trim();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var projections = XmlConverter.Deserializer<ProjectionXmlInputModel>(xmlString, "Projections");
            foreach (var currentProjection in projections)
            {
                bool isValidDateTime = DateTime.TryParseExact(
                    currentProjection.DateTime,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime currentDateTime);

                var currentMovie = context.Movies.FirstOrDefault(m => m.Id == currentProjection.MovieId);
                //var currentHall = context.Halls.FirstOrDefault(h => h.Id == currentProjection.HallId);

                if (!IsValid(currentProjection) ||
                    !isValidDateTime ||
                    currentMovie == null ||
                    !context.Halls.Any(h=>h.Id == currentProjection.HallId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projectionToAdd = new Projection
                {
                    MovieId = currentProjection.MovieId,
                    HallId = currentProjection.HallId,                  
                    DateTime = currentDateTime
                };

                context.Projections.Add(projectionToAdd);
                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfulImportProjection, projectionToAdd.Movie.Title, projectionToAdd.DateTime.ToString("MM/dd/yyyy")));
            }
            
            return sb.ToString().Trim();

        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var customers = XmlConverter.Deserializer<CustomerXmlInputModel>(xmlString, "Customers");
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
                context.Customers.Add(customerToAdd);
                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customerToAdd.FirstName, customerToAdd.LastName, customerToAdd.Tickets.Count));
            }
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