namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var writers = JsonConvert.DeserializeObject<IEnumerable<WriterJsonInputModel>>(jsonString);

            foreach (var currentWriter in writers)
            {
                if (!IsValid(currentWriter))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var writerToAdd = new Writer
                {
                    Name = currentWriter.Name,
                    Pseudonym = currentWriter.Pseudonym
                };

                context.Writers.Add(writerToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedWriter, writerToAdd.Name));
                context.SaveChanges();
            }

            return sb.ToString().Trim();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var producers = JsonConvert.DeserializeObject<IEnumerable<ProducerJsonInputModel>>(jsonString);
            foreach (var currentProducer in producers)
            {
                if (!IsValid(currentProducer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var producerToAdd = new Producer
                {
                    Name = currentProducer.Name,
                    Pseudonym = currentProducer.Pseudonym,
                    PhoneNumber = currentProducer.PhoneNumber
                };

                bool AllAlbumsValid = true;

                foreach (var currentAlbum in currentProducer.Albums)
                {
                    bool isValidDate = DateTime.TryParseExact(
                        currentAlbum.ReleaseDate,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                         out DateTime currentReleaseDate);
                    if (!IsValid(currentAlbum) ||
                        !isValidDate)
                    {
                        AllAlbumsValid = false;
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    var album = new Album
                    {
                        Name = currentAlbum.Name,
                        ReleaseDate = currentReleaseDate
                    };

                    producerToAdd.Albums.Add(album);
                }
                if (AllAlbumsValid)
                {
                    context.Producers.Add(producerToAdd);
                    context.SaveChanges();
                    string message = producerToAdd.PhoneNumber != null
                        ? string.Format(SuccessfullyImportedProducerWithPhone,
                            producerToAdd.Name, producerToAdd.PhoneNumber, producerToAdd.Albums.Count)
                        : string.Format(SuccessfullyImportedProducerWithNoPhone,
                            producerToAdd.Name, producerToAdd.Albums.Count);
                    sb.AppendLine(message);
                }
            }
            return sb.ToString().Trim();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var songs = XmlConverter.Deserializer<SongXmlInputModel>(xmlString, "Songs");

            foreach (var currentSong in songs)
            {
                bool isValidCreatedOn = DateTime.TryParseExact(currentSong.CreatedOn,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime currentCreatedOn);

                bool isValidDuration = TimeSpan.TryParseExact(currentSong.Duration,
                    "c", CultureInfo.InvariantCulture,
                    TimeSpanStyles.None,
                    out TimeSpan currentDuration);

                bool isValidGenre = Enum.TryParse<Genre>(currentSong.Genre, out Genre currentGenre);

                //проверка дали currentSong.AlbumId не е null
                var currentAlbum = context.Albums.FirstOrDefault(a => a.Id == currentSong.AlbumId);

                var currentWriter = context.Writers.FirstOrDefault(w => w.Id == currentSong.WriterId);

                if (!IsValid(currentSong) ||
                    !isValidCreatedOn ||
                    !isValidDuration ||
                    !isValidGenre ||
                    currentAlbum == null ||
                    currentWriter == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var songToAdd = new Song
                {
                    Name = currentSong.Name,
                    Duration = currentDuration,
                    CreatedOn = currentCreatedOn,
                    Genre = currentGenre,
                    Album = currentAlbum,
                    Writer = currentWriter,
                    Price = currentSong.Price
                };

                context.Songs.Add(songToAdd);
                context.SaveChanges();
                sb.AppendLine(string.Format(SuccessfullyImportedSong, songToAdd.Name, songToAdd.Genre, songToAdd.Duration));
            }
            return sb.ToString().Trim();
        }


        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var performers = XmlConverter.Deserializer<PerformerXmlInputModel>(xmlString, "Performers");
            foreach (var currentPerformer in performers)
            {
                if (!IsValid(currentPerformer))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var performerToAdd = new Performer
                {
                    FirstName = currentPerformer.FirstName,
                    LastName = currentPerformer.LastName,
                    Age = currentPerformer.Age,
                    NetWorth = currentPerformer.NetWorth
                };

                bool SongsValid = true;
                foreach (var currentSong in currentPerformer.PerformersSongs)
                {
                    var songToAdd = context.Songs.FirstOrDefault(s => s.Id == currentSong.Id);
                    if(songToAdd == null)
                    {
                        SongsValid = false;
                        sb.AppendLine(ErrorMessage);
                        break;
                    }
                    performerToAdd.PerformerSongs.Add(new SongPerformer
                    {
                        Song = songToAdd
                    });
                }
                if (SongsValid)
                {
                    context.Performers.Add(performerToAdd);
                    context.SaveChanges();
                    sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performerToAdd.FirstName, performerToAdd.PerformerSongs.Count));
                }
            }
            return sb.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}