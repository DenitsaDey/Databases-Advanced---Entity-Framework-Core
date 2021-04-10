namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres = context.Genres
				.ToList()
				.Where(g => genreNames.Contains(g.Name))
				.Select(g => new
				{
					Id = g.Id,
					Genre = g.Name,
					Games = g.Games.Where(ga => ga.Purchases.Any())
							.Select(ga => new
							{
								Id = ga.Id,
								Title = ga.Name,
								Developer = ga.Developer.Name,
								Tags = string.Join(", ", ga.GameTags.Select(gt => gt.Tag.Name).ToArray()),
								Players = ga.Purchases.Count
							})
							.OrderByDescending(ga => ga.Players)
							.ThenBy(ga => ga.Id)
							.ToList(),
					TotalPlayers = g.Games.Sum(ga => ga.Purchases.Count)
				})
				.OrderByDescending(g => g.TotalPlayers)
				.ThenBy(g => g.Id)
				.ToList();

			var jsonResult = JsonConvert.SerializeObject(genres, Formatting.Indented);

			return jsonResult;
           
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			//PurchaseType purchaseType = Enum.Parse<PurchaseType>(storeType);

			var users = context.Users
				.ToArray()
				.Where(u => u.Cards.Any(c => c.Purchases.Any(p=>p.Type.ToString() == storeType)))
				.Select(u=>new ExportUserDto
                {
					Username = u.Username,
					Purchases = u.Cards
								.SelectMany(c => c.Purchases)
								.Where(p=>p.Type.ToString() == storeType)
								.Select(p=>new PurchaseDto
								{
									Card = p.Card.Number,
									Cvc = p.Card.Cvc,
									Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
									Game = new GameDto
									{
										Title = p.Game.Name,
										Genre = p.Game.Genre.Name,
										Price = p.Game.Price
									}
								})
								.OrderBy(p=>p.Date)
								.ToArray(),
                    TotalSpent = u.Cards
                                    .Sum(c => c.Purchases.Where(p => p.Type.ToString() == storeType)
                                            .Sum(p => p.Game.Price))

                })
				.OrderByDescending(u=>u.TotalSpent)
				.ThenBy(u=>u.Username)
                .ToArray();

			//XmlSerializer xmlSerializer = 
			//	new XmlSerializer(typeof(ExportUserDto[]), 
			//		new XmlRootAttribute("Users"));
			//var sw = new StringWriter();
			//xmlSerializer.Serialize(sw, users);
			//         return sw.ToString().TrimEnd();

			var xml = XmlConverter.Serialize(users, "Users");
			return xml;
        } 
	}
}