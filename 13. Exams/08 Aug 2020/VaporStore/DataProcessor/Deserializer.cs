namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		private const string ErrorMessage = "Invalid Data";

		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder sb = new StringBuilder();

			var games = JsonConvert.DeserializeObject<IEnumerable<GameInputModel>>(jsonString);

            foreach (var jsonGame in games)
            {
				bool isDateValid = DateTime.TryParseExact(jsonGame.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
					DateTimeStyles.None, out DateTime releaseDate);

				if (!IsValid(jsonGame) ||
					!jsonGame.Tags.Any() ||
					!isDateValid)
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				var genre = context.Genres.FirstOrDefault(g => g.Name == jsonGame.Genre);
				if(genre == null)
                {
					genre = new Genre { Name = jsonGame.Genre };
                }

				var developer = context.Developers.FirstOrDefault(d => d.Name == jsonGame.Developer);
				if (developer == null)
				{
					developer = new Developer { Name = jsonGame.Developer };
				}
				//var developer = context.Developers
				//						.FirstOrDefault(d => d.Name == jsonGame.Developer) ??
				//						new Developer { Name = jsonGame.Developer };


				var gameToImport = new Game
				{
					Name = jsonGame.Name,
					Price = jsonGame.Price,
					ReleaseDate = releaseDate,
					Developer = developer,
					Genre = genre
				};

				foreach (var jsonTag in jsonGame.Tags)
				{
					var tag = context.Tags.FirstOrDefault(t => t.Name == jsonTag) ?? new Tag { Name = jsonTag };
					gameToImport.GameTags.Add(new GameTag { Tag = tag });
				}
				context.Games.Add(gameToImport);
				context.SaveChanges();
				sb.AppendLine($"Added {jsonGame.Name} ({jsonGame.Genre}) with {jsonGame.Tags.Count()} tags");
            }
			
			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder sb = new StringBuilder();
			var users = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(jsonString);

            foreach (var jsonUser in users)
            {
                
				if (!IsValid(jsonUser) || 
					!jsonUser.Cards.Any() ||
					!jsonUser.Cards.All(IsValid))
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				var userToImport = new User
				{
					FullName = jsonUser.FullName,
					Username = jsonUser.Username,
					Email = jsonUser.Email,
					Age = jsonUser.Age
				};
				foreach (var userCard in jsonUser.Cards)
				{
					bool isCardValid = Enum.TryParse(userCard.Type, out CardType type);
					if (!isCardValid)
					{
						sb.AppendLine(ErrorMessage);
						break;
					}

					var card = new Card
					{
						Number = userCard.Number,
						Cvc = userCard.CVC,
						Type = type,
						User = userToImport
					};
					userToImport.Cards.Add(card);
				}
				context.Users.Add(userToImport);
				context.SaveChanges();
				sb.AppendLine($"Imported {userToImport.Username} with {userToImport.Cards.Count} cards");
			}

			return sb.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			StringBuilder sb = new StringBuilder();
			var purchasesToImport = XmlConverter.Deserializer<PurchaseInputModel>(xmlString, "Purchases");

			foreach (var purchase in purchasesToImport)
            {

				var isValidDate = DateTime.TryParseExact(
					purchase.Date,
					"dd/MM/yyyy HH:mm",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out DateTime currentDate);

				if (!IsValid(purchase) || !isValidDate)
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				Card currentCard = context.Cards.FirstOrDefault(c => c.Number == purchase.Card);
				if(currentCard == null)
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				Game currentGame = context.Games.FirstOrDefault(g => g.Name == purchase.Title);
				if(currentGame == null)
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

				var purchaseToAdd = new Purchase
				{
					Type = Enum.Parse<PurchaseType>(purchase.Type),
					ProductKey = purchase.ProductKey,
					Date = currentDate,
					Card = currentCard,
					Game = currentGame
				};
				context.Purchases.Add(purchaseToAdd);
				sb.AppendLine($"Imported {currentGame.Name} for {purchaseToAdd.Card.User.Username}");
            }

			context.SaveChanges();

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