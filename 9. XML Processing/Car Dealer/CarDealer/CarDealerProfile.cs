using AutoMapper;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDto, Supplier>();
            this.CreateMap<ImportPartDto, Part>();
            this.CreateMap<ImportCustomerDto, Customer>();
            this.CreateMap<ImportSaleDto, Sale>();

            this.CreateMap<Supplier, ExportLocalSuppliersDto>();
            this.CreateMap<Car, ExportCarWithDistanceDto>();
            this.CreateMap<Car, ExportCarBMWDto>();
            this.CreateMap<Part, ExportCarPartDto>();
            this.CreateMap<Car, ExportCarDto>()
                .ForMember(x => x.Parts,
                            y => y.MapFrom(x => x.PartCars
                                               .Select(pc => pc.Part)));
            this.CreateMap<Customer, ExportCustomerWithOneBoughtCarDto>()
                .ForMember(c => c.BoughtCars, y => y.MapFrom(s => s.Sales.Count))
                .ForMember(c => c.SpentMoney,
                    y => y.MapFrom(c => c.Sales.Select(s => s.Car.PartCars.Select(pc => pc.Part).Sum(pc => pc.Price)).Sum()));

            this.CreateMap<Car, ExportCarAttributesDto>();
            this.CreateMap<Sale, ExportSaleDto>()
                .ForMember(s => s.Car, y => y.MapFrom(s => s.Car))
                .ForMember(s => s.CustomerName, y => y.MapFrom(s => s.Customer.Name))
                .ForMember(s => s.Discount, y => y.MapFrom(s => s.Discount))
                .ForMember(s => s.Price, y => y.MapFrom(s => s.Car.PartCars.Select(pc => pc.Part.Price).Sum()))
                .ForMember(s => s.PriceWithDiscount,
                            y => y.MapFrom(s => s.Car.PartCars.Select(pc => pc.Part.Price).Sum() -
                                            s.Discount / 100 * s.Car.PartCars.Select(pc => pc.Part.Price).Sum()));
        }
    }
}
