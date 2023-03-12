using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<CarDto, Car>()
                .ForMember(x => x.TraveledDistance, y => y.MapFrom(s => s.TraveledDistance));
        }
    }
}
