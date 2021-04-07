using Newtonsoft.Json;
using PetClinic.Data;
using PetClinic.DataProcessor.Dto.Export;
using System.Globalization;
using System.Linq;

namespace PetClinic.DataProcessor
{
    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals.ToList()
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(a => new
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.PassportSerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                })
                .OrderByDescending(x=>x.Age)
                .ThenBy(x=>x.SerialNumber)
                .ToList();

            string json = JsonConvert.SerializeObject(animals, Formatting.Indented);
            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var procedures = context.Procedures
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Animal.PassportSerialNumber)
                .Select(p=> new ProcedureXmlExportModel
                {
                    PassportSerialNumber = p.Animal.PassportSerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.ProcedureAnimalAids
                                .Select(x => new AnimalAidXmlExportModel
                                {
                                    Name = x.AnimalAid.Name,
                                    Price = x.AnimalAid.Price
                                })
                                .ToArray(),
                    TotalPrice = p.Cost
                })
                .ToArray();

            string xml = XmlConverter.Serialize(procedures, "Procedures");
            return xml;
        }
    }
}