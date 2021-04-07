using Newtonsoft.Json;
using PetClinic.Data;
using PetClinic.Data.Models;
using PetClinic.DataProcessor.Dto.Import;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PetClinic.DataProcessor
{
    public class Deserializer
    {
        private const string SuccessMessage = "Record {0} successfully imported.";
        private const string SuccessMessageAnimal = "Record {0} Passport №: {1} successfully imported.";
        private const string SuccessMessageProcedure = "Record successfully imported.";
        private const string ErrorMessage = "Error: Invalid data.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var animalAidsToImport = new List<AnimalAid>();

            var animalAidDtos = JsonConvert.DeserializeObject<IEnumerable<AnimalAidJsonInputModel>>(jsonString);
            foreach (var currentAnimalAid in animalAidDtos)
            {
                if (!IsValid(currentAnimalAid))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var animalAidtoAdd = new AnimalAid
                {
                    Name = currentAnimalAid.Name,
                    Price = currentAnimalAid.Price
                };

                if (animalAidsToImport.Any(a=>a.Name == animalAidtoAdd.Name))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                animalAidsToImport.Add(animalAidtoAdd);
                sb.AppendLine(string.Format(SuccessMessage, animalAidtoAdd.Name));
            }
            context.AnimalAids.AddRange(animalAidsToImport);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var animalsToImport = new List<Animal>();
            var passports = new List<Passport>();

            var animals = JsonConvert.DeserializeObject<IEnumerable<AnimalJsonInputModel>>(jsonString);
            foreach (var currentAnimal in animals)
            {
                bool isValidRegistrationDate = DateTime.TryParseExact(
                    currentAnimal.Passport.RegistrationDate,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime currentRegistrationDate);

                if (!IsValid(currentAnimal) ||
                    !isValidRegistrationDate ||
                    !IsValid(currentAnimal.Passport) ||
                    passports.Any(p=>p.SerialNumber == currentAnimal.Passport.SerialNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var animalToAdd = new Animal
                {
                    Name = currentAnimal.Name,
                    Type = currentAnimal.Type,
                    Age = currentAnimal.Age,
                };

                var currentPassport = new Passport
                {
                    Animal = animalToAdd,
                    SerialNumber = currentAnimal.Passport.SerialNumber,
                    OwnerName = currentAnimal.Passport.OwnerName,
                    OwnerPhoneNumber = currentAnimal.Passport.OwnerPhoneNumber,
                    RegistrationDate = currentRegistrationDate
                };

                animalToAdd.Passport = currentPassport;
                animalToAdd.PassportSerialNumber = currentPassport.SerialNumber;
                passports.Add(currentPassport);
                animalsToImport.Add(animalToAdd);
                sb.AppendLine(string.Format(SuccessMessageAnimal, animalToAdd.Name, animalToAdd.PassportSerialNumber));
            }
            context.Animals.AddRange(animalsToImport);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var validVets = new List<Vet>();

            var vets = XmlConverter.Deserializer<VetXmlInputModel>(xmlString, "Vets");
            foreach (var currentVet in vets)
            {
                if (!IsValid(currentVet) ||
                    validVets.Any(vv=>vv.PhoneNumber == currentVet.PhoneNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var vetToAdd = new Vet
                {
                    Name = currentVet.Name,
                    Profession = currentVet.Profession,
                    Age = currentVet.Age,
                    PhoneNumber = currentVet.PhoneNumber
                };

                validVets.Add(vetToAdd);
                sb.AppendLine(string.Format(SuccessMessage, vetToAdd.Name));
            }

            context.Vets.AddRange(validVets);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var validProcedures = new List<Procedure>();

            var procedures = XmlConverter.Deserializer<ProcedureXmlInputModel>(xmlString, "Procedures");

            foreach (var currentProcedure in procedures)
            {
                if (!IsValid(currentProcedure))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var currentVet = context.Vets.FirstOrDefault(v => v.Name == currentProcedure.VetName);
                var currentAnimal = context.Animals.FirstOrDefault(a => a.Passport.SerialNumber == currentProcedure.Animal);
                bool isValidDateTime = DateTime.TryParseExact(
                   currentProcedure.DateTime,
                   "dd-MM-yyyy",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out DateTime currentDateTime);

                if (currentVet == null || 
                    currentAnimal == null ||
                    !isValidDateTime)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var procedureToAdd = new Procedure
                {
                    Vet = currentVet,
                    Animal = currentAnimal,
                    DateTime = currentDateTime
                };

                foreach (var animalaid in currentProcedure.AnimalAids)
                {
                    if (!IsValid(animalaid))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var currentAnimalAid = context.AnimalAids.FirstOrDefault(aa => aa.Name == animalaid.Name);
                    var existingAnimalAidProcedure = procedureToAdd.ProcedureAnimalAids.FirstOrDefault(paa => paa.AnimalAid.Name == currentAnimalAid.Name);
                    //bool existsAnimalAidProcedure = procedureToAdd.ProcedureAnimalAids.Any(paa => paa.AnimalAid.Name == currentAnimalAid.Name);

                    if (currentAnimalAid == null ||
                        existingAnimalAidProcedure != null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    procedureToAdd.ProcedureAnimalAids.Add(new ProcedureAnimalAid { AnimalAid = currentAnimalAid, Procedure = procedureToAdd });
                }
                validProcedures.Add(procedureToAdd);
                sb.AppendLine(SuccessMessageProcedure);
            }
            context.Procedures.AddRange(validProcedures);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}