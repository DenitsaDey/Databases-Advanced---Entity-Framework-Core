using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string DatasetsDirPath = @"../../../Datasets/";
        private const string ResultDirPath = DatasetsDirPath + "Results/";
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<CarDealerProfile>(); });

            using var db = new CarDealerContext();
            //ResetDatabase(db);

            //9.Import Suppliers
            //string inputXml = File.ReadAllText(DatasetsDirPath + "suppliers.xml");
            //string result = ImportSuppliers(db, inputXml);
            //Console.WriteLine(result);

            //10.Import Parts
            //string inputXml = File.ReadAllText(DatasetsDirPath + "parts.xml");
            //string result = ImportParts(db, inputXml);
            //Console.WriteLine(result);

            //11. Import Cars
            //string inputXml = File.ReadAllText(DatasetsDirPath + "cars.xml");
            //string result = ImportCars(db, inputXml);
            //Console.WriteLine(result);

            //12. Import Customers
            //string inputXml = File.ReadAllText(DatasetsDirPath + "customers.xml");
            //string result = ImportCustomers(db, inputXml);
            //Console.WriteLine(result);

            //13. Import Sales
            //string inputXml = File.ReadAllText(DatasetsDirPath + "sales.xml");
            //string result = ImportSales(db, inputXml);
            //Console.WriteLine(result);

            //14. Export Cars with Travelled Distance
            //string result = GetCarsWithDistance(db);
            //File.WriteAllText(ResultDirPath + "cars.xml", result);

            ////15. Export Cars from Make BMW
            //string result = GetCarsFromMakeBmw(db);
            //File.WriteAllText(ResultDirPath + "bmw-cars.xml", result);

            //16. Export Local Suppliers
            //string result = GetLocalSuppliers(db);
            //File.WriteAllText(ResultDirPath + "local-suppliers.xml", result);

            //17. Export Cars with Their Parts
            //string result = GetCarsWithTheirListOfParts(db);
            //File.WriteAllText(ResultDirPath + "cars-and-parts.xml", result);

            ////18. Export Total Sales By Cistomer
            //string result = GetTotalSalesByCustomer(db);
            //File.WriteAllText(ResultDirPath + "customers-total-sales.xml", result);

            //19. Export Sales With Discount
            string result = GetSalesWithAppliedDiscount(db);
            File.WriteAllText(ResultDirPath + "sales-discounts.xml", result);


        }

        //9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));

            ImportSupplierDto[] supplierDtos;

            using (var reader = new StringReader(inputXml))
            {
                supplierDtos = (ImportSupplierDto[])xmlSerializer.Deserialize(reader);
            }

            List<Supplier> suppliers = new List<Supplier>();
            foreach (var dto in supplierDtos)
            {
                Supplier supplier = new Supplier()
                {
                    Name = dto.Name,
                    IsImporter = dto.IsImporter
                };
                suppliers.Add(supplier);
            }
            //when using mapper:
            //var suppliers = Mapper.Map<Supplier[]>(supplierDtos);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));

            ImportPartDto[] partDtos;

            using (var reader = new StringReader(inputXml))
            {
                partDtos = ((ImportPartDto[])xmlSerializer
                    .Deserialize(reader))
                    .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                    .ToArray();
            }

            List<Part> parts = new List<Part>();
            foreach (var dto in partDtos)
            {
                Part part = new Part
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    SupplierId = dto.SupplierId
                };
                parts.Add(part);
            }
            //var parts = Mapper.Map<Part[]>(partDtos);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //11. Impoert Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarsDto[]), new XmlRootAttribute("Cars"));

            ImportCarsDto[] carsDtos;

            using (var reader = new StringReader(inputXml))
            {
                carsDtos = ((ImportCarsDto[])xmlSerializer
                    .Deserialize(reader));
            }

            List<Car> cars = new List<Car>();
            List<PartCar> partCars = new List<PartCar>();

            foreach (var dto in carsDtos)
            {
                Car car = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TravelledDistance = dto.TravelledDistance,
                };

                var parts = dto.Parts
                    .Where(pc => context.Parts.Any(p => p.Id == pc.Id))
                    .Select(p => p.Id)
                    .Distinct();

                foreach (var part in parts)
                {
                    PartCar partCar = new PartCar()
                    {
                        PartId = part,
                        Car = car
                    };
                    partCars.Add(partCar);
                }
                cars.Add(car);
            }
            //var parts = Mapper.Map<Part[]>(partDtos);

            context.PartCars.AddRange(partCars);
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            ImportCustomerDto[] customerDtos = (ImportCustomerDto[])xmlSerializer
                                                                    .Deserialize(new StringReader(inputXml));

            List<Customer> customers = new List<Customer>();
            foreach (var dto in customerDtos)
            {
                DateTime date;
                bool isValidDate = DateTime.TryParseExact(dto.BirthDate, "yyyy-MM-dd'T'HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                if (isValidDate)
                {

                    Customer customer = new Customer
                    {
                        Name = dto.Name,
                        BirthDate = date,
                        IsYoungDriver = dto.IsYoungDriver
                    };
                    customers.Add(customer);
                }
            }

            //var customers = Mapper.Map<Customer[]>(customerDtos);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));

            ImportSaleDto[] saleDtos = (ImportSaleDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Sale> sales = new List<Sale>();
            foreach (var dto in saleDtos)
            {
                if(context.Cars.Any(c=>c.Id == dto.CarId))
                {
                    Sale sale = new Sale()
                    {
                        CarId = dto.CarId,
                        CustomerId = dto.CustomerId,
                        Discount = dto.Discount
                    };

                    sales.Add(sale);
                }
            }
           
            //var sales = Mapper.Map<Sale[]>(saleDtos).Where(s => context.Cars.Any(c => c.Id == s.CarId)).ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();

           return $"Successfully imported {sales.Count}";
        }

        //14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var carsWithDistance = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new ExportCarWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistnace = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCarWithDistanceDto[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(new StringWriter(sb), carsWithDistance, namespaces);

            return sb.ToString().TrimEnd();
        }

        //15. Export Cars From Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var carsBMW = context
                .Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportCarBMWDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();


            var xmlSerializer = new XmlSerializer(typeof(ExportCarBMWDto[]), new XmlRootAttribute("cars"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(new StringWriter(sb), carsBMW, namespaces);

            return sb.ToString().TrimEnd();
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var localSuppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new ExportLocalSuppliersDto     //.ProjectTo<ExportLocalSuppliersDto>()
                {                                           //
                    Id = s.Id,                              //
                    Name = s.Name,                          //
                    PartsCount = s.Parts.Count              //
                })                                          //
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportLocalSuppliersDto[]), new XmlRootAttribute("suppliers"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(new StringWriter(sb), localSuppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        //17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var cars = context
                .Cars
                .Select(c => new ExportCarDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new ExportCarPartDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var xmlSerializer = new XmlSerializer(typeof(ExportCarDto[]), new XmlRootAttribute("cars"));
            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //18. Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var customers = context
                .Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new ExportCustomerWithOneBoughtCarDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                    //c.Sales.Select(s => s.Car.PartCars.Select(pc => pc.Part).Sum(pc=>pc.Price)).Sum()

                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportCustomerWithOneBoughtCarDto[]), new XmlRootAttribute("customers"));

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().Trim();
        }

        //19. Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var sales = context
                .Sales
                .Select(s => new ExportSaleDto()
                {
                    Car = new ExportCarAttributesDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistnace = s.Car.TravelledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) -
                     s.Discount / 100 * s.Car.PartCars.Sum(pc => pc.Part.Price)

                })
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportSaleDto[]), new XmlRootAttribute("sales"));

            xmlSerializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().Trim();
        }
        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database successfully deleted!");
            db.Database.EnsureCreated();
            Console.WriteLine("Database successfully created!");
        }
    }
}